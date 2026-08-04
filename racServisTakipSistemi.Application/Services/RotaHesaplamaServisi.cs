using AracServisTakipSistemi.Application.DTOs;
using AracServisTakipSistemi.Application.Interfaces;
using AracServisTakipSistemi.Domain.Entities;
using AracServisTakipSistemi.Domain.Enums;

namespace AracServisTakipSistemi.Application.Services;

public class RotaHesaplamaServisi
{
    private readonly IMesafeHesaplayici _mesafeHesaplayici;
    private const double OrtalamaHizKmSaat = 35.0;

    public RotaHesaplamaServisi(IMesafeHesaplayici mesafeHesaplayici)
    {
        _mesafeHesaplayici = mesafeHesaplayici;
    }

    public RotaHesaplamaSonucu RotalariHesapla(
        List<Personel> aktifPersoneller,
        List<Arac> aktifAraclar,
        List<Bolge> aktifBolgeler,
        List<Vardiya> aktifVardiyalar,
        Dictionary<int, PersonelAdres> personelAdresSozlugu)
    {
        var genelSonuc = new RotaHesaplamaSonucu();
        var calisanlar = aktifPersoneller.Where(p => p.PersonelTuru == PersonelTuru.Calisan).ToList();

        var bolgesizPersonel = calisanlar.Where(p => p.BolgeId == null).ToList();
        if (bolgesizPersonel.Count > 0)
            genelSonuc.Uyarilar.Add($"{bolgesizPersonel.Count} personel hiçbir bölgeye atanmamış.");

        foreach (var vardiya in aktifVardiyalar)
        {
            var vardiyaPersonelleri = calisanlar.Where(p => p.VardiyaId == vardiya.Id && p.BolgeId != null).ToList();
            if (vardiyaPersonelleri.Count == 0) continue;

            var vardiyaSonucu = TekVardiyaIcinHesapla(vardiyaPersonelleri, aktifAraclar, aktifBolgeler, vardiya, personelAdresSozlugu);

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
        Dictionary<int, PersonelAdres> adresSozlugu)
    {
        var sonuc = new RotaHesaplamaSonucu();

        var koordinatiEksik = personeller.Where(p => !adresSozlugu.ContainsKey(p.Id)
            || adresSozlugu[p.Id].Enlem == null || adresSozlugu[p.Id].Boylam == null).ToList();
        var gecerliPersoneller = personeller.Except(koordinatiEksik).ToList();
        sonuc.KoordinatiEksikPersoneller = koordinatiEksik;

        if (gecerliPersoneller.Count == 0) return sonuc;

        // Kullanılabilir araçlar: aktif, şoförü olan, şoförün koordinatı olan
        var kullanilabilirAraclar = araclar.Where(a =>
            a.SoforPersonelId.HasValue &&
            adresSozlugu.ContainsKey(a.SoforPersonelId.Value) &&
            adresSozlugu[a.SoforPersonelId.Value].Enlem != null).ToList();

        foreach (var arac in araclar.Except(kullanilabilirAraclar))
            sonuc.Uyarilar.Add($"'{arac.Plaka}' plakalı aracın şoförü/koordinatı tanımlı değil, kullanılamadı.");

        // Bölge başına personel grubu
        var bolgePersonelleri = gecerliPersoneller
            .GroupBy(p => p.BolgeId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var kalanBolgeIdleri = new List<int>(bolgePersonelleri.Keys);
        var kullanilmisAraclar = new HashSet<int>();

        while (kalanBolgeIdleri.Count > 0)
        {
            // En kalabalık bölgeden başla (seed)
            var seedBolgeId = kalanBolgeIdleri.OrderByDescending(id => bolgePersonelleri[id].Count).First();
            var seedBolge = bolgeler.First(b => b.Id == seedBolgeId);

            var grupBolgeIdleri = new List<int> { seedBolgeId };
            kalanBolgeIdleri.Remove(seedBolgeId);
            var grupPersonel = new List<Personel>(bolgePersonelleri[seedBolgeId]);

            // Eşiğin altındaysa komşu bölgelerle birleştir
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

            // Bu gruba araç(lar) ata — kapasite + tampon kontrolü, gerekirse böl
            var musaitAraclar = kullanilabilirAraclar.Where(a => !kullanilmisAraclar.Contains(a.Id)).ToList();
            var tamponluKapasite = musaitAraclar.Sum(a => a.KapasiteSayisi) > 0
                ? musaitAraclar.Max(a => a.KapasiteSayisi) + seedBolge.KapasiteTamponu
                : 0;

            var kalanGrupPersonel = new List<Personel>(grupPersonel);

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

                var buArayaAlinacaklar = kalanGrupPersonel.Take(uygunArac.KapasiteSayisi + seedBolge.KapasiteTamponu).ToList();
                kullanilmisAraclar.Add(uygunArac.Id);
                kalanGrupPersonel = kalanGrupPersonel.Skip(buArayaAlinacaklar.Count).ToList();

                var soforAdresi = adresSozlugu[uygunArac.SoforPersonelId!.Value];
                var soforKonumu = (soforAdresi.Enlem!.Value, soforAdresi.Boylam!.Value);

                var siraliListe = SiraOptimizeEt(buArayaAlinacaklar, soforKonumu, adresSozlugu);
                var toplamSureDk = ToplamRotaSuresiHesapla(siraliListe, soforKonumu, adresSozlugu);

                foreach (var p in buArayaAlinacaklar) p.ServisDurumu = ServisDurumu.Atanmis;

                sonuc.Rotalar.Add(new RotaSonucu
                {
                    AracId = uygunArac.Id,
                    Plaka = uygunArac.Plaka,
                    VardiyaId = vardiya.Id,
                    VardiyaAdi = vardiya.VardiyaAdi,
                    BolgeIdleri = grupBolgeIdleri,
                    ZiyaretSirasi = siraliListe,
                    TahminiToplamSureDk = (int)toplamSureDk,
                    GidisKalkisSaati = vardiya.BaslangicSaati.Subtract(TimeSpan.FromMinutes(toplamSureDk)),
                    DonusKalkisSaati = vardiya.BitisSaati
                });
            }
        }

        return sonuc;
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

    private double ToplamRotaSuresiHesapla(List<Personel> siraliListe, (double Enlem, double Boylam) baslangic, Dictionary<int, PersonelAdres> adresSozlugu)
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

        return toplamKm / OrtalamaHizKmSaat * 60;
    }
}