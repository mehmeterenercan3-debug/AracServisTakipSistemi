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
    private readonly double _maksimumRotaYayilmaKm;
    private const double OrtalamaHizKmSaat = 35.0;

    public RotaHesaplamaServisi(IMesafeHesaplayici mesafeHesaplayici, IOptions<RotaAyarlari> rotaAyarlari)
    {
        _mesafeHesaplayici = mesafeHesaplayici;
        _maksimumRotaYayilmaKm = rotaAyarlari.Value.MaksimumRotaYayilmaKm;
    }

    public RotaHesaplamaSonucu RotalariHesapla(
        List<Personel> aktifPersoneller,
        List<Arac> aktifAraclar,
        List<Vardiya> aktifVardiyalar)
    {
        var genelSonuc = new RotaHesaplamaSonucu();

        var calisanlarTumu = aktifPersoneller.Where(p => p.PersonelTuru == PersonelTuru.Calisan).ToList();

        var vardiyasizPersonel = calisanlarTumu.Where(p => p.VardiyaId == null).ToList();
        if (vardiyasizPersonel.Count > 0)
        {
            genelSonuc.Uyarilar.Add($"{vardiyasizPersonel.Count} personelin vardiyası atanmamış, rotaya dahil edilemedi.");
        }

        foreach (var vardiya in aktifVardiyalar)
        {
            var vardiyaPersonelleri = calisanlarTumu.Where(p => p.VardiyaId == vardiya.Id).ToList();
            if (vardiyaPersonelleri.Count == 0) continue;

            var vardiyaSonucu = TekVardiyaIcinHesapla(vardiyaPersonelleri, aktifAraclar, vardiya);

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

    private RotaHesaplamaSonucu TekVardiyaIcinHesapla(List<Personel> personeller, List<Arac> araclar, Vardiya vardiya)
    {
        var sonuc = new RotaHesaplamaSonucu();

        var koordinatiEksik = personeller.Where(p => p.Enlem == null || p.Boylam == null).ToList();
        var rotayaGirebilenler = personeller.Where(p => p.Enlem != null && p.Boylam != null).ToList();
        sonuc.KoordinatiEksikPersoneller = koordinatiEksik;

        if (rotayaGirebilenler.Count == 0 || araclar.Count == 0) return sonuc;

        var kullanilabilirAraclar = new List<Arac>();
        foreach (var arac in araclar)
        {
            if (arac.SoforPersonel == null || arac.SoforPersonel.Enlem == null || arac.SoforPersonel.Boylam == null)
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
            .Select(a => (a.SoforPersonel!.Enlem!.Value, a.SoforPersonel!.Boylam!.Value))
            .ToList();

        var kumeler = Enumerable.Range(0, kullanilabilirAraclar.Count).Select(_ => new List<Personel>()).ToList();

        for (int iter = 0; iter < 8; iter++)
        {
            kumeler = Enumerable.Range(0, kullanilabilirAraclar.Count).Select(_ => new List<Personel>()).ToList();
            foreach (var p in rotayaGirebilenler.OrderBy(_ => Guid.NewGuid()))
            {
                int enIyi = -1;
                double enKucuk = double.MaxValue;
                for (int k = 0; k < merkezler.Count; k++)
                {
                    if (kumeler[k].Count >= kullanilabilirAraclar[k].KapasiteSayisi) continue;
                    var d = _mesafeHesaplayici.MesafeHesaplaKm(p.Enlem!.Value, p.Boylam!.Value, merkezler[k].Item1, merkezler[k].Item2);
                    if (d < enKucuk) { enKucuk = d; enIyi = k; }
                }
                if (enIyi >= 0) kumeler[enIyi].Add(p);
            }
        }

        for (int k = 0; k < kullanilabilirAraclar.Count; k++)
        {
            var soforEvi = (kullanilabilirAraclar[k].SoforPersonel!.Enlem!.Value, kullanilabilirAraclar[k].SoforPersonel!.Boylam!.Value);
            var siraliListe = SiraOptimizeEt(kumeler[k], soforEvi);
            var toplamSureDk = ToplamRotaSuresiHesapla(siraliListe, soforEvi);

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

    private List<Personel> SiraOptimizeEt(List<Personel> personeller, (double Enlem, double Boylam) baslangicNoktasi)
    {
        if (personeller.Count == 0) return personeller;

        var kalanlar = new List<Personel>(personeller);
        var sirali = new List<Personel>();

        var ilkPersonel = kalanlar
            .OrderBy(p => _mesafeHesaplayici.MesafeHesaplaKm(baslangicNoktasi.Enlem, baslangicNoktasi.Boylam, p.Enlem!.Value, p.Boylam!.Value))
            .First();

        sirali.Add(ilkPersonel);
        kalanlar.Remove(ilkPersonel);

        while (kalanlar.Count > 0)
        {
            var son = sirali[^1];
            var enYakin = kalanlar
                .OrderBy(p => _mesafeHesaplayici.MesafeHesaplaKm(son.Enlem!.Value, son.Boylam!.Value, p.Enlem!.Value, p.Boylam!.Value))
                .First();
            sirali.Add(enYakin);
            kalanlar.Remove(enYakin);
        }

        return sirali;
    }

    private double ToplamRotaSuresiHesapla(List<Personel> ziyaretSirasi, (double Enlem, double Boylam) baslangicNoktasi)
    {
        if (ziyaretSirasi.Count == 0) return 0;

        double toplamKm = _mesafeHesaplayici.MesafeHesaplaKm(
            baslangicNoktasi.Enlem, baslangicNoktasi.Boylam,
            ziyaretSirasi[0].Enlem!.Value, ziyaretSirasi[0].Boylam!.Value);

        for (int i = 0; i < ziyaretSirasi.Count - 1; i++)
            toplamKm += _mesafeHesaplayici.MesafeHesaplaKm(
                ziyaretSirasi[i].Enlem!.Value, ziyaretSirasi[i].Boylam!.Value,
                ziyaretSirasi[i + 1].Enlem!.Value, ziyaretSirasi[i + 1].Boylam!.Value);

        return toplamKm / OrtalamaHizKmSaat * 60;
    }
}