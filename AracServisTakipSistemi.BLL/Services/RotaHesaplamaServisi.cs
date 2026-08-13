using AracServisTakipSistemi.BLL.DTOs;
using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Entities.Enums;

namespace AracServisTakipSistemi.BLL.Services;

public class RotaHesaplamaServisi
{
    private readonly IMesafeHesaplayici _mesafeHesaplayici;
    private readonly ISirketAyarRepository _sirketAyarRepository;
    private const double OrtalamaHizKmSaat = 35.0;
    private const double DurakBeklemeSuresiDk = 3.0;

    // Bir aracı doldururken, sıradaki iki kişi arasında bu mesafeden (km) büyük bir sıçrama varsa,
    // "burada farklı bir küme başlıyor" kabul edip aracı kapasiteye kadar doldurmaya zorlamıyoruz.
    private const double DogalBoslukEsigiKm = 15.0;

    public RotaHesaplamaServisi(IMesafeHesaplayici mesafeHesaplayici, ISirketAyarRepository sirketAyarRepository)
    {
        _mesafeHesaplayici = mesafeHesaplayici;
        _sirketAyarRepository = sirketAyarRepository;
    }

    public async Task<RotaHesaplamaSonucu> RotalariHesaplaAsync(
        List<Personel> aktifPersoneller,
        List<Arac> aktifAraclar,
        List<Bolge> aktifBolgeler,
        List<Vardiya> aktifVardiyalar,
        Dictionary<int, PersonelAdres> personelAdresSozlugu)
    {
        var ayarlar = await _sirketAyarRepository.GetirAsync();

        var genelSonuc = new RotaHesaplamaSonucu();
        var calisanlar = aktifPersoneller.Where(p => p.PersonelTuru == PersonelTuru.Calisan).ToList();

        var bolgesizPersonel = calisanlar.Where(p => p.BolgeId == null).ToList();
        if (bolgesizPersonel.Count > 0)
            genelSonuc.Uyarilar.Add($"{bolgesizPersonel.Count} personel hiçbir bölgeye atanmamış.");

        if (ayarlar.Enlem == 0 && ayarlar.Boylam == 0)
            genelSonuc.Uyarilar.Add("Şirket konumu tanımlanmamış (Admin > Ayarlar ekranından girin) — rota hesaplaması hatalı olabilir.");

        foreach (var vardiya in aktifVardiyalar)
        {
            var vardiyaPersonelleri = calisanlar.Where(p => p.VardiyaId == vardiya.Id && p.BolgeId != null).ToList();
            if (vardiyaPersonelleri.Count == 0) continue;

            var vardiyaSonucu = TekVardiyaIcinHesapla(vardiyaPersonelleri, aktifAraclar, aktifBolgeler, vardiya, personelAdresSozlugu, ayarlar);

            genelSonuc.Rotalar.AddRange(vardiyaSonucu.Rotalar);
            genelSonuc.BeklemedeKalanPersoneller.AddRange(vardiyaSonucu.BeklemedeKalanPersoneller);
            genelSonuc.KoordinatiEksikPersoneller.AddRange(vardiyaSonucu.KoordinatiEksikPersoneller);
            genelSonuc.Uyarilar.AddRange(vardiyaSonucu.Uyarilar);

            if (vardiyaSonucu.KapasiteYetersiz)
            {
                genelSonuc.KapasiteYetersiz = true;
                genelSonuc.OnerilenEkAracSayisi = Math.Max(genelSonuc.OnerilenEkAracSayisi, vardiyaSonucu.OnerilenEkAracSayisi);
            }
        }

        return genelSonuc;
    }

    private RotaHesaplamaSonucu TekVardiyaIcinHesapla(
        List<Personel> personeller,
        List<Arac> araclar,
        List<Bolge> bolgeler,
        Vardiya vardiya,
        Dictionary<int, PersonelAdres> adresSozlugu,
        SirketAyar ayarlar)
    {
        var sonuc = new RotaHesaplamaSonucu();
        var sirketKonumu = (ayarlar.Enlem, ayarlar.Boylam);

        var koordinatiEksik = personeller.Where(p => !adresSozlugu.ContainsKey(p.Id)
            || adresSozlugu[p.Id].Enlem == null || adresSozlugu[p.Id].Boylam == null).ToList();
        var gecerliPersoneller = personeller.Except(koordinatiEksik).ToList();
        sonuc.KoordinatiEksikPersoneller = koordinatiEksik;

        if (gecerliPersoneller.Count == 0) return sonuc;

        var kullanilabilirAraclar = araclar.Where(a =>
            a.SoforPersonelId.HasValue &&
            adresSozlugu.ContainsKey(a.SoforPersonelId.Value) &&
            adresSozlugu[a.SoforPersonelId.Value].Enlem != null)
            .OrderByDescending(a => a.KapasiteSayisi)   // büyük kapasiteli araçlar önce kullanılsın — en kalabalık ilk küme büyük araca düşsün
            .ToList();

        foreach (var arac in araclar.Except(kullanilabilirAraclar))
            sonuc.Uyarilar.Add($"'{arac.Plaka}' plakalı aracın şoförü/koordinatı tanımlı değil, kullanılamadı.");

        var bolgePersonelleri = gecerliPersoneller
            .GroupBy(p => p.BolgeId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var kalanBolgeIdleri = new List<int>(bolgePersonelleri.Keys);
        var kullanilmisAraclar = new HashSet<int>();

        while (kalanBolgeIdleri.Count > 0)
        {
            var seedBolgeId = kalanBolgeIdleri.OrderByDescending(id => bolgePersonelleri[id].Count).First();
            var seedBolge = bolgeler.First(b => b.Id == seedBolgeId);

            var grupBolgeIdleri = new List<int> { seedBolgeId };
            kalanBolgeIdleri.Remove(seedBolgeId);
            var grupPersonel = new List<Personel>(bolgePersonelleri[seedBolgeId]);

            while (grupPersonel.Count < seedBolge.MinPersonelEsigi && kalanBolgeIdleri.Count > 0)
            {
                var seedMerkez = (seedBolge.MerkezEnlem, seedBolge.MerkezBoylam);
                var enYakinKomsu = kalanBolgeIdleri
                    .Where(id => bolgeler.First(b => b.Id == id).MerkezEnlem.HasValue)
                    .OrderBy(id => _mesafeHesaplayici.MesafeHesaplaKm(
                        seedMerkez.MerkezEnlem!.Value, seedMerkez.MerkezBoylam!.Value,
                        bolgeler.First(b => b.Id == id).MerkezEnlem!.Value, bolgeler.First(b => b.Id == id).MerkezBoylam!.Value))
                    .FirstOrDefault();

                if (enYakinKomsu == 0 && !kalanBolgeIdleri.Contains(0)) break;
                if (!kalanBolgeIdleri.Contains(enYakinKomsu)) break;

                grupBolgeIdleri.Add(enYakinKomsu);
                grupPersonel.AddRange(bolgePersonelleri[enYakinKomsu]);
                kalanBolgeIdleri.Remove(enYakinKomsu);
            }

            var musaitAraclar = kullanilabilirAraclar.Where(a => !kullanilmisAraclar.Contains(a.Id)).ToList();

            // Araçlara bölmeden ÖNCE tüm bölge grubunu coğrafi olarak (bölge merkezinden başlayarak)
            // tek bir zincire diziyoruz — böylece ardışık parçalara bölündüğünde
            // birbirine yakın kişiler aynı araca düşer, rastgele/liste sırasına göre değil.
            var kalanGrupPersonel = seedBolge.MerkezEnlem.HasValue && seedBolge.MerkezBoylam.HasValue
                ? SiraOptimizeEt(grupPersonel, (seedBolge.MerkezEnlem.Value, seedBolge.MerkezBoylam.Value), adresSozlugu)
                : new List<Personel>(grupPersonel);

            while (kalanGrupPersonel.Count > 0)
            {
                var uygunArac = musaitAraclar.FirstOrDefault(a => !kullanilmisAraclar.Contains(a.Id));

                if (uygunArac == null)
                {
                    sonuc.KapasiteYetersiz = true;
                    sonuc.BeklemedeKalanPersoneller.AddRange(kalanGrupPersonel);
                    foreach (var p in kalanGrupPersonel) p.ServisDurumu = ServisDurumu.Beklemede;

                    var ortalamaKapasite = araclar.Count > 0 ? araclar.Average(a => a.KapasiteSayisi) : 14;
                    sonuc.OnerilenEkAracSayisi = Math.Max(sonuc.OnerilenEkAracSayisi,
                        (int)Math.Ceiling(kalanGrupPersonel.Count / ortalamaKapasite));
                    sonuc.Uyarilar.Add($"'{seedBolge.BolgeAdi}' bölgesi için yeterli araç yok, {kalanGrupPersonel.Count} kişi beklemede kaldı.");
                    break;
                }

                var buArayaAlinacaklar = KapasiteyeGoreDogalGrupAl(kalanGrupPersonel, uygunArac.KapasiteSayisi, adresSozlugu);
                kullanilmisAraclar.Add(uygunArac.Id);
                kalanGrupPersonel = kalanGrupPersonel.Skip(buArayaAlinacaklar.Count).ToList();

                var soforAdresi = adresSozlugu[uygunArac.SoforPersonelId!.Value];
                var soforKonumu = (soforAdresi.Enlem!.Value, soforAdresi.Boylam!.Value);

                foreach (var p in buArayaAlinacaklar) p.ServisDurumu = ServisDurumu.Atanmis;

                // ================= GİDİŞ (sabah): şoförün evi -> personeller -> ŞİRKET =================
                var gidisSirasi = SiraOptimizeEt(buArayaAlinacaklar, soforKonumu, adresSozlugu);
                var gidisKumulatifDk = KumulatifSureHesapla(gidisSirasi, soforKonumu, adresSozlugu);
                var gidisKumulatifKm = KumulatifMesafeHesapla(gidisSirasi, soforKonumu, adresSozlugu);

                var sonPersonelAdresi = adresSozlugu[gidisSirasi[^1].Id];
                var sonBacakKm = _mesafeHesaplayici.MesafeHesaplaKm(
                    sonPersonelAdresi.Enlem!.Value, sonPersonelAdresi.Boylam!.Value,
                    sirketKonumu.Item1, sirketKonumu.Item2);
                var sonBacakDk = sonBacakKm / OrtalamaHizKmSaat * 60;

                var gidisToplamKm = gidisKumulatifKm + sonBacakKm;
                var gidisToplamDk = gidisKumulatifDk[^1] + sonBacakDk;

                // Şirkete, vardiya başlangıcından X dk önce varılmalı → kalkış saati geriye doğru hesaplanır
                var sirketVarisSaati = vardiya.BaslangicSaati.Subtract(TimeSpan.FromMinutes(ayarlar.GidisVarisTamponDk));
                var gidisKalkisSaati = sirketVarisSaati.Subtract(TimeSpan.FromMinutes(gidisToplamDk));
                var gidisVarisSaatleri = gidisKumulatifDk.Select(dk => gidisKalkisSaati.Add(TimeSpan.FromMinutes(dk))).ToList();

                sonuc.Rotalar.Add(new RotaSonucu
                {
                    AracId = uygunArac.Id,
                    Plaka = uygunArac.Plaka,
                    VardiyaId = vardiya.Id,
                    VardiyaAdi = vardiya.VardiyaAdi,
                    Yon = RotaYonu.Gidis,
                    BolgeIdleri = grupBolgeIdleri,
                    ZiyaretSirasi = gidisSirasi,
                    VarisSaatleri = gidisVarisSaatleri,
                    ToplamMesafeKm = gidisToplamKm,
                    TahminiToplamSureDk = (int)gidisToplamDk,
                    KalkisSaati = gidisKalkisSaati
                });

                // ================= DÖNÜŞ (akşam): ŞİRKET -> personeller (ters sırayla) =================
                var donusSirasi = new List<Personel>(gidisSirasi);
                donusSirasi.Reverse();

                var donusKumulatifDk = KumulatifSureHesapla(donusSirasi, sirketKonumu, adresSozlugu);
                var donusKumulatifKm = KumulatifMesafeHesapla(donusSirasi, sirketKonumu, adresSozlugu);

                var donusKalkisSaati = vardiya.BitisSaati.Add(TimeSpan.FromMinutes(ayarlar.DonusKalkisTamponDk));
                var donusVarisSaatleri = donusKumulatifDk.Select(dk => donusKalkisSaati.Add(TimeSpan.FromMinutes(dk))).ToList();

                sonuc.Rotalar.Add(new RotaSonucu
                {
                    AracId = uygunArac.Id,
                    Plaka = uygunArac.Plaka,
                    VardiyaId = vardiya.Id,
                    VardiyaAdi = vardiya.VardiyaAdi,
                    Yon = RotaYonu.Donus,
                    BolgeIdleri = grupBolgeIdleri,
                    ZiyaretSirasi = donusSirasi,
                    VarisSaatleri = donusVarisSaatleri,
                    ToplamMesafeKm = donusKumulatifKm,
                    TahminiToplamSureDk = (int)(donusKumulatifDk.Count > 0 ? donusKumulatifDk[^1] : 0),
                    KalkisSaati = donusKalkisSaati
                });
            }
        }

        return sonuc;
    }

    // Coğrafi olarak sıralı bir listeden, kapasiteye kadar kişi alır — ama araya "doğal bir boşluk"
    // (DogalBoslukEsigiKm'den büyük bir sıçrama) girerse, kapasiteyi doldurmaya zorlamadan orada durur.
    // Bu, birbirinden uzak adreslerin sırf kapasiteyi doldurmak için aynı araca tıkılmasını önler.
    private List<Personel> KapasiteyeGoreDogalGrupAl(List<Personel> siraliListe, int kapasite, Dictionary<int, PersonelAdres> adresSozlugu)
    {
        if (siraliListe.Count <= kapasite) return new List<Personel>(siraliListe);

        var aday = siraliListe.Take(kapasite).ToList();

        for (int i = 1; i < aday.Count; i++)
        {
            var onceki = adresSozlugu[aday[i - 1].Id];
            var simdiki = adresSozlugu[aday[i].Id];
            var mesafe = _mesafeHesaplayici.MesafeHesaplaKm(
                onceki.Enlem!.Value, onceki.Boylam!.Value, simdiki.Enlem!.Value, simdiki.Boylam!.Value);

            if (mesafe > DogalBoslukEsigiKm)
                return aday.Take(i).ToList();   // doğal boşluk bulundu, burada kes
        }

        return aday;   // hiç doğal boşluk yok, kapasiteyi tam doldur
    }

    private List<Personel> SiraOptimizeEt(List<Personel> personeller, (double Enlem, double Boylam) baslangic, Dictionary<int, PersonelAdres> adresSozlugu)
    {
        if (personeller.Count == 0) return personeller;
        var kalanlar = new List<Personel>(personeller);
        var sirali = new List<Personel>();

        var ilk = kalanlar.OrderBy(p => _mesafeHesaplayici.MesafeHesaplaKm(baslangic.Enlem, baslangic.Boylam, adresSozlugu[p.Id].Enlem!.Value, adresSozlugu[p.Id].Boylam!.Value)).First();
        sirali.Add(ilk);
        kalanlar.Remove(ilk);

        while (kalanlar.Count > 0)
        {
            var sonAdres = adresSozlugu[sirali[^1].Id];
            var enYakin = kalanlar.OrderBy(p => _mesafeHesaplayici.MesafeHesaplaKm(sonAdres.Enlem!.Value, sonAdres.Boylam!.Value, adresSozlugu[p.Id].Enlem!.Value, adresSozlugu[p.Id].Boylam!.Value)).First();
            sirali.Add(enYakin);
            kalanlar.Remove(enYakin);
        }

        return sirali;
    }

    // Başlangıç noktasından (şoför evi ya da şirket) her durağa varana kadar geçen kümülatif süre (dk)
    private List<double> KumulatifSureHesapla(
        List<Personel> siraliListe,
        (double Enlem, double Boylam) baslangic,
        Dictionary<int, PersonelAdres> adresSozlugu)
    {
        var sonuc = new List<double>();

        if (siraliListe.Count == 0)
            return sonuc;

        var ilkAdres = adresSozlugu[siraliListe[0].Id];

        double kumulatifKm = _mesafeHesaplayici.MesafeHesaplaKm(
            baslangic.Enlem,
            baslangic.Boylam,
            ilkAdres.Enlem!.Value,
            ilkAdres.Boylam!.Value);

        double kumulatifDk = kumulatifKm / OrtalamaHizKmSaat * 60;

        sonuc.Add(kumulatifDk);

        for (int i = 0; i < siraliListe.Count - 1; i++)
        {
            var a1 = adresSozlugu[siraliListe[i].Id];
            var a2 = adresSozlugu[siraliListe[i + 1].Id];

            kumulatifKm += _mesafeHesaplayici.MesafeHesaplaKm(
                a1.Enlem!.Value,
                a1.Boylam!.Value,
                a2.Enlem!.Value,
                a2.Boylam!.Value);

            kumulatifDk =
                kumulatifKm / OrtalamaHizKmSaat * 60
                + ((i + 1) * DurakBeklemeSuresiDk);

            sonuc.Add(kumulatifDk);
        }

        return sonuc;
    }

    private double KumulatifMesafeHesapla(List<Personel> siraliListe, (double Enlem, double Boylam) baslangic, Dictionary<int, PersonelAdres> adresSozlugu)
    {
        if (siraliListe.Count == 0) return 0;
        var ilkAdres = adresSozlugu[siraliListe[0].Id];
        double toplamKm = _mesafeHesaplayici.MesafeHesaplaKm(baslangic.Enlem, baslangic.Boylam, ilkAdres.Enlem!.Value, ilkAdres.Boylam!.Value);

        for (int i = 0; i < siraliListe.Count - 1; i++)
        {
            var a1 = adresSozlugu[siraliListe[i].Id];
            var a2 = adresSozlugu[siraliListe[i + 1].Id];
            toplamKm += _mesafeHesaplayici.MesafeHesaplaKm(a1.Enlem!.Value, a1.Boylam!.Value, a2.Enlem!.Value, a2.Boylam!.Value);
        }

        return toplamKm;
    }
}