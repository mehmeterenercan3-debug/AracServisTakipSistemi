using AracServisTakipSistemi.Application.DTOs;
using AracServisTakipSistemi.Application.Interfaces;
using AracServisTakipSistemi.Application.Options;
using AracServisTakipSistemi.Domain.Entities;
using AracServisTakipSistemi.Domain.Enums;
using Microsoft.Extensions.Options;

namespace AracServisTakipSistemi.Application.Services;

public class RotaHesaplamaServisi
{
    private readonly IMesafeHesaplayici _mesafeHesaplayici;
    private readonly IPersonelAdresRepository _personelAdresRepository;
    private readonly double _maksimumRotaYayilmaKm;
    private const double OrtalamaHizKmSaat = 35.0;

    public RotaHesaplamaServisi(
        IMesafeHesaplayici mesafeHesaplayici,
        IPersonelAdresRepository personelAdresRepository,
        IOptions<RotaAyarlari> rotaAyarlari)
    {
        _mesafeHesaplayici = mesafeHesaplayici;
        _personelAdresRepository = personelAdresRepository;
        _maksimumRotaYayilmaKm = rotaAyarlari.Value.MaksimumRotaYayilmaKm;
    }

    public async Task<RotaHesaplamaSonucu> RotalariHesaplaAsync(
        List<Personel> aktifPersoneller,
        List<Arac> aktifAraclar,
        List<Vardiya> aktifVardiyalar)
    {
        var genelSonuc = new RotaHesaplamaSonucu();

        var calisanlarTumu = aktifPersoneller.Where(p => p.PersonelTuru == PersonelTuru.Calisan).ToList();

        var tumIlgiliIdler = calisanlarTumu.Select(p => p.Id)
            .Concat(aktifAraclar.Where(a => a.SoforPersonelId.HasValue).Select(a => a.SoforPersonelId!.Value))
            .Distinct()
            .ToList();

        var adresSozlugu = await _personelAdresRepository.AktifAdresleriGetirAsync(tumIlgiliIdler);

        var vardiyasizPersonel = calisanlarTumu.Where(p => p.VardiyaId == null).ToList();
        if (vardiyasizPersonel.Count > 0)
        {
            genelSonuc.Uyarilar.Add($"{vardiyasizPersonel.Count} personelin vardiyası atanmamış, rotaya dahil edilemedi.");
        }

        foreach (var vardiya in aktifVardiyalar)
        {
            var vardiyaPersonelleri = calisanlarTumu.Where(p => p.VardiyaId == vardiya.Id).ToList();
            if (vardiyaPersonelleri.Count == 0) continue;

            var vardiyaSonucu = TekVardiyaIcinHesapla(vardiyaPersonelleri, aktifAraclar, vardiya, adresSozlugu);

            genelSonuc.AracRotalari.AddRange(vardiyaSonucu.AracRotalari);
            genelSonuc.AtanamayanPersoneller.AddRange(vardiyaSonucu.AtanamayanPersoneller);
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
        Vardiya vardiya,
        Dictionary<int, PersonelAdres> adresSozlugu)
    {
        var sonuc = new RotaHesaplamaSonucu();

        var koordinatiEksik = personeller.Where(p => !adresSozlugu.ContainsKey(p.Id)
            || adresSozlugu[p.Id].Enlem == null || adresSozlugu[p.Id].Boylam == null).ToList();
        var rotayaGirebilenler = personeller.Except(koordinatiEksik).ToList();
        sonuc.KoordinatiEksikPersoneller = koordinatiEksik;

        if (rotayaGirebilenler.Count == 0 || araclar.Count == 0) return sonuc;

        var kullanilabilirAraclar = new List<Arac>();
        foreach (var arac in araclar)
        {
            var soforVar = arac.SoforPersonelId.HasValue
                && adresSozlugu.TryGetValue(arac.SoforPersonelId.Value, out var soforAdresi)
                && soforAdresi.Enlem != null && soforAdresi.Boylam != null;

            if (!soforVar)
            {
                sonuc.Uyarilar.Add($"'{arac.Plaka}' plakalı aracın şoförü veya şoförün adres koordinatı tanımlı değil, bu araç rotaya dahil edilemedi.");
                continue;
            }
            kullanilabilirAraclar.Add(arac);
        }

        if (kullanilabilirAraclar.Count == 0) return sonuc;

        int toplamKapasite = kullanilabilirAraclar.Sum(a => a.KapasiteSayisi);
        if (rotayaGirebilenler.Count > toplamKapasite)
        {
            int eksik = rotayaGirebilenler.Count - toplamKapasite;
            sonuc.KapasiteYetersiz = true;
            sonuc.OnerilenEkAracSayisi = (int)Math.Ceiling(eksik / kullanilabilirAraclar.Average(a => a.KapasiteSayisi));
            sonuc.Uyarilar.Add($"'{vardiya.VardiyaAdi}' vardiyasında {eksik} kişilik kapasite eksik.");
        }

        var merkezler = kullanilabilirAraclar
            .Select(a => (adresSozlugu[a.SoforPersonelId!.Value].Enlem!.Value, adresSozlugu[a.SoforPersonelId!.Value].Boylam!.Value))
            .ToList();

        var kumeler = Enumerable.Range(0, kullanilabilirAraclar.Count).Select(_ => new List<Personel>()).ToList();

        for (int iter = 0; iter < 8; iter++)
        {
            kumeler = Enumerable.Range(0, kullanilabilirAraclar.Count).Select(_ => new List<Personel>()).ToList();
            foreach (var p in rotayaGirebilenler.OrderBy(_ => Guid.NewGuid()))
            {
                var pAdres = adresSozlugu[p.Id];
                int enIyi = -1;
                double enKucuk = double.MaxValue;
                for (int k = 0; k < merkezler.Count; k++)
                {
                    if (kumeler[k].Count >= kullanilabilirAraclar[k].KapasiteSayisi) continue;
                    var d = _mesafeHesaplayici.MesafeHesaplaKm(pAdres.Enlem!.Value, pAdres.Boylam!.Value, merkezler[k].Item1, merkezler[k].Item2);
                    if (d < enKucuk) { enKucuk = d; enIyi = k; }
                }
                if (enIyi >= 0) kumeler[enIyi].Add(p);
            }
        }

        for (int k = 0; k < kullanilabilirAraclar.Count; k++)
        {
            var soforAdresi = adresSozlugu[kullanilabilirAraclar[k].SoforPersonelId!.Value];
            var soforKonumu = (soforAdresi.Enlem!.Value, soforAdresi.Boylam!.Value);

            var siraliListe = SiraOptimizeEt(kumeler[k], soforKonumu, adresSozlugu);
            var toplamSureDk = ToplamRotaSuresiHesapla(siraliListe, soforKonumu, adresSozlugu);

            sonuc.AracRotalari.Add(new AracRotaSonucu
            {
                AracId = kullanilabilirAraclar[k].Id,
                Plaka = kullanilabilirAraclar[k].Plaka,
                VardiyaId = vardiya.Id,
                VardiyaAdi = vardiya.VardiyaAdi,
                ZiyaretSirasi = siraliListe,
                TahminiToplamSureDk = (int)toplamSureDk,
                GidisKalkisSaati = vardiya.BaslangicSaati.Subtract(TimeSpan.FromMinutes(toplamSureDk)),
                DonusKalkisSaati = vardiya.BitisSaati
            });
        }

        var atananlar = sonuc.AracRotalari.SelectMany(a => a.ZiyaretSirasi).Select(p => p.Id).ToHashSet();
        sonuc.AtanamayanPersoneller = rotayaGirebilenler.Where(p => !atananlar.Contains(p.Id)).ToList();

        return sonuc;
    }

    private List<Personel> SiraOptimizeEt(List<Personel> personeller, (double Enlem, double Boylam) baslangicNoktasi, Dictionary<int, PersonelAdres> adresSozlugu)
    {
        if (personeller.Count == 0) return personeller;

        var kalanlar = new List<Personel>(personeller);
        var sirali = new List<Personel>();

        var ilkPersonel = kalanlar
            .OrderBy(p => _mesafeHesaplayici.MesafeHesaplaKm(baslangicNoktasi.Enlem, baslangicNoktasi.Boylam, adresSozlugu[p.Id].Enlem!.Value, adresSozlugu[p.Id].Boylam!.Value))
            .First();

        sirali.Add(ilkPersonel);
        kalanlar.Remove(ilkPersonel);

        while (kalanlar.Count > 0)
        {
            var son = sirali[^1];
            var sonAdres = adresSozlugu[son.Id];
            var enYakin = kalanlar
                .OrderBy(p => _mesafeHesaplayici.MesafeHesaplaKm(sonAdres.Enlem!.Value, sonAdres.Boylam!.Value, adresSozlugu[p.Id].Enlem!.Value, adresSozlugu[p.Id].Boylam!.Value))
                .First();
            sirali.Add(enYakin);
            kalanlar.Remove(enYakin);
        }

        return sirali;
    }

    private double ToplamRotaSuresiHesapla(List<Personel> ziyaretSirasi, (double Enlem, double Boylam) baslangicNoktasi, Dictionary<int, PersonelAdres> adresSozlugu)
    {
        if (ziyaretSirasi.Count == 0) return 0;

        var ilkAdres = adresSozlugu[ziyaretSirasi[0].Id];
        double toplamKm = _mesafeHesaplayici.MesafeHesaplaKm(
            baslangicNoktasi.Enlem, baslangicNoktasi.Boylam,
            ilkAdres.Enlem!.Value, ilkAdres.Boylam!.Value);

        for (int i = 0; i < ziyaretSirasi.Count - 1; i++)
        {
            var a1 = adresSozlugu[ziyaretSirasi[i].Id];
            var a2 = adresSozlugu[ziyaretSirasi[i + 1].Id];
            toplamKm += _mesafeHesaplayici.MesafeHesaplaKm(a1.Enlem!.Value, a1.Boylam!.Value, a2.Enlem!.Value, a2.Boylam!.Value);
        }

        return toplamKm / OrtalamaHizKmSaat * 60;
    }
}