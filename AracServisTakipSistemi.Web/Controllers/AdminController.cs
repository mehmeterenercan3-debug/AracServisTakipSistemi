using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.Entities.Enums;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly PersonelServisi _personelServisi;
    private readonly AracServisi _aracServisi;
    private readonly BolgeServisi _bolgeServisi;

    public AdminController(PersonelServisi personelServisi, AracServisi aracServisi, BolgeServisi bolgeServisi)
    {
        _personelServisi = personelServisi;
        _aracServisi = aracServisi;
        _bolgeServisi = bolgeServisi;
    }

    public async Task<IActionResult> Index()
    {
        var personeller = await _personelServisi.AktifPersonelleriGetirAsync();
        var araclar = await _aracServisi.TumAraclariGetirAsync();
        var bolgeler = await _bolgeServisi.AktifBolgeleriGetirAsync();

        var sinirTarih = DateTime.Now.AddDays(-30);

        // Her aracın son 30 gündeki EN SON hesaplaması — aynı araç birden fazla kez
        // hesaplanmışsa listede/grafikte tekrar tekrar görünüp karışıklık yaratmasın diye.
        var sonRiskler = araclar
            .Select(a => new
            {
                Arac = a,
                Risk = a.RiskSkorlari
                    .Where(r => r.HesaplamaTarihi >= sinirTarih)
                    .OrderByDescending(r => r.HesaplamaTarihi)
                    .FirstOrDefault()
            })
            .Where(x => x.Risk != null)
            .Select(x => new { x.Arac, Risk = x.Risk! })
            .OrderByDescending(x => x.Risk.HesaplamaTarihi)
            .ToList();

        var model = new AdminDashboardViewModel
        {
            ToplamAktifPersonel = personeller.Count,
            BeklemedeKalanPersonel = personeller.Count(p => p.ServisDurumu == ServisDurumu.Beklemede),
            ToplamAktifArac = araclar.Count(a => a.AktifMi),
            BakimdakiAracSayisi = araclar.Count(a => a.BakimdaMi),
            ToplamBolge = bolgeler.Count,
            RiskDagilimi = SonOtuzGunRiskDagilimiHesapla(sonRiskler.Select(x => x.Risk).ToList()),
            BolgePersonelDagilimi = BolgePersonelDagilimiHesapla(personeller, bolgeler),
            SonRiskHesaplamalari = sonRiskler.Select(x => new RiskDetayi
            {
                Plaka = x.Arac.Plaka,
                Seviye = SeviyeMetni(x.Risk.RiskSeviyesi),
                Puan = x.Risk.SkorDegeri,
                Tarih = x.Risk.HesaplamaTarihi
            }).ToList()
        };

        return View(model);
    }

    private static string SeviyeMetni(BakimRiskSeviyesi seviye) => seviye switch
    {
        BakimRiskSeviyesi.Dusuk => "Düşük",
        BakimRiskSeviyesi.Orta => "Orta",
        BakimRiskSeviyesi.Yuksek => "Yüksek",
        BakimRiskSeviyesi.Kritik => "Kritik",
        _ => seviye.ToString()
    };

    // Son 30 günde hesaplanan risk skorlarını seviyeye göre sayıyor.
    // Sabit bir sırada (Düşük→Kritik) döndürüyor, veri yoksa 0 gösteriyor.
    private List<GrafikVeriNoktasi> SonOtuzGunRiskDagilimiHesapla(List<Entities.Entities.RiskSkoru> sonRiskler)
    {
        var siraliSeviyeler = new[] { BakimRiskSeviyesi.Dusuk, BakimRiskSeviyesi.Orta, BakimRiskSeviyesi.Yuksek, BakimRiskSeviyesi.Kritik };

        return siraliSeviyeler.Select(seviye => new GrafikVeriNoktasi
        {
            Etiket = SeviyeMetni(seviye),
            Deger = sonRiskler.Count(r => r.RiskSeviyesi == seviye)
        }).ToList();
    }

    // Her aktif bölgedeki aktif personel sayısı — bölgesi olmayanlar "Atanmadı" olarak ayrı gösteriliyor.
    private List<GrafikVeriNoktasi> BolgePersonelDagilimiHesapla(List<Entities.Entities.Personel> personeller, List<Entities.Entities.Bolge> bolgeler)
    {
        var sonuc = bolgeler.Select(b => new GrafikVeriNoktasi
        {
            Etiket = b.BolgeAdi,
            Deger = personeller.Count(p => p.BolgeId == b.Id)
        }).Where(v => v.Deger > 0).ToList();

        var atanmamisSayisi = personeller.Count(p => p.BolgeId == null);
        if (atanmamisSayisi > 0)
            sonuc.Add(new GrafikVeriNoktasi { Etiket = "Atanmadı", Deger = atanmamisSayisi });

        return sonuc;
    }
}