using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Entities.Enums;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AracController : Controller
{
    private readonly AracServisi _aracServisi;
    private readonly PersonelServisi _personelServisi;
    private readonly BakimRiskServisi _bakimRiskServisi;
    private readonly AracArizaKaydiServisi _arizaKaydiServisi;
    private readonly BakimKaydiServisi _bakimKaydiServisi;

    public AracController(
        AracServisi aracServisi,
        PersonelServisi personelServisi,
        BakimRiskServisi bakimRiskServisi,
        AracArizaKaydiServisi arizaKaydiServisi,
        BakimKaydiServisi bakimKaydiServisi)
    {
        _aracServisi = aracServisi;
        _personelServisi = personelServisi;
        _bakimRiskServisi = bakimRiskServisi;
        _arizaKaydiServisi = arizaKaydiServisi;
        _bakimKaydiServisi = bakimKaydiServisi;
    }

    public async Task<IActionResult> Index()
    {
        var araclar = await _aracServisi.TumAraclariGetirAsync();
        var tumPersonel = await _personelServisi.AktifPersonelleriGetirAsync();
        ViewBag.Soforler = tumPersonel.Where(p => p.PersonelTuru == PersonelTuru.Sofor).ToList();
        return View(araclar);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [FromForm] string Plaka,
        [FromForm] string Marka,
        [FromForm] string Model,
        [FromForm] string AracTipi,
        [FromForm] int ModelYili,
        [FromForm] double GuncelKm,
        [FromForm] DateTime SatinAlmaTarihi,
        [FromForm] int KapasiteSayisi,
        [FromForm] bool BakimdaMi,
        [FromForm] string? SasiNo,
        [FromForm] string? MotorNo,
        [FromForm] DateTime? MuayeneTarihi,
        [FromForm] DateTime? SigortaBitisTarihi,
        [FromForm] int? SoforPersonelId,
        [FromForm] bool AktifMi)
    {
        if (string.IsNullOrWhiteSpace(Plaka) || string.IsNullOrWhiteSpace(Marka) ||
            string.IsNullOrWhiteSpace(Model) || string.IsNullOrWhiteSpace(AracTipi))
        {
            TempData["Hata"] = "Plaka, Marka, Model ve Araç Tipi zorunludur.";
            return RedirectToAction(nameof(Index));
        }

        var arac = new Arac
        {
            Plaka = Plaka,
            Marka = Marka,
            Model = Model,
            AracTipi = AracTipi,
            ModelYili = ModelYili,
            GuncelKm = GuncelKm,
            SatinAlmaTarihi = SatinAlmaTarihi,
            KapasiteSayisi = KapasiteSayisi,
            BakimdaMi = BakimdaMi,
            SasiNo = SasiNo,
            MotorNo = MotorNo,
            MuayeneTarihi = MuayeneTarihi,
            SigortaBitisTarihi = SigortaBitisTarihi,
            SoforPersonelId = SoforPersonelId,
            AktifMi = AktifMi
        };

        await _aracServisi.AracEkleAsync(arac);
        TempData["Basari"] = "Araç başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        [FromForm] int Id,
        [FromForm] string Plaka,
        [FromForm] string Marka,
        [FromForm] string Model,
        [FromForm] string AracTipi,
        [FromForm] int ModelYili,
        [FromForm] double GuncelKm,
        [FromForm] DateTime SatinAlmaTarihi,
        [FromForm] int KapasiteSayisi,
        [FromForm] bool BakimdaMi,
        [FromForm] string? SasiNo,
        [FromForm] string? MotorNo,
        [FromForm] DateTime? MuayeneTarihi,
        [FromForm] DateTime? SigortaBitisTarihi,
        [FromForm] int? SoforPersonelId,
        [FromForm] bool AktifMi)
    {
        if (string.IsNullOrWhiteSpace(Plaka) || string.IsNullOrWhiteSpace(Marka) ||
            string.IsNullOrWhiteSpace(Model) || string.IsNullOrWhiteSpace(AracTipi))
        {
            TempData["Hata"] = "Plaka, Marka, Model ve Araç Tipi zorunludur.";
            return RedirectToAction(nameof(Index));
        }

        var arac = await _aracServisi.AracGetirAsync(Id);
        if (arac == null) return NotFound();

        // Güncel km, aracın en yüksek arıza-anındaki-km değerinden düşük olamaz —
        // yoksa geçmişte "gelecekte yaşanmış" bir arıza kaydı gibi tutarsız bir durum oluşur.
        if (arac.ArizaKayitlari.Any())
        {
            var enYuksekArizaKm = arac.ArizaKayitlari.Max(a => a.ArizaAnindakiKm);
            if (GuncelKm < enYuksekArizaKm)
            {
                TempData["Hata"] = $"Güncel km ({GuncelKm:N0}), bu araca ait bir arıza kaydındaki km'den ({enYuksekArizaKm:N0}) düşük olamaz. " +
                    "Lütfen km değerini kontrol edin.";
                return RedirectToAction(nameof(Index));
            }
        }

        arac.Plaka = Plaka;
        arac.Marka = Marka;
        arac.Model = Model;
        arac.AracTipi = AracTipi;
        arac.ModelYili = ModelYili;
        arac.GuncelKm = GuncelKm;
        arac.SatinAlmaTarihi = SatinAlmaTarihi;
        arac.KapasiteSayisi = KapasiteSayisi;
        arac.BakimdaMi = BakimdaMi;
        arac.SasiNo = SasiNo;
        arac.MotorNo = MotorNo;
        arac.MuayeneTarihi = MuayeneTarihi;
        arac.SigortaBitisTarihi = SigortaBitisTarihi;
        arac.SoforPersonelId = SoforPersonelId;
        arac.AktifMi = AktifMi;

        await _aracServisi.AracGuncelleAsync(arac);
        TempData["Basari"] = "Araç başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Sil(int id)
    {
        var silindiMi = await _aracServisi.AracSilAsync(id);
        return Json(silindiMi ? "başarılı" : "hata");
    }

    [HttpPost]
    public async Task<IActionResult> DurumDegistir(int id)
    {
        var arac = await _aracServisi.AracGetirAsync(id);
        if (arac == null)
            return Json("hata");

        arac.AktifMi = !arac.AktifMi;
        await _aracServisi.AracGuncelleAsync(arac);
        return Json("başarılı");
    }

    [HttpPost]
    public async Task<IActionResult> RiskHesapla(int id)
    {
        try
        {
            var sonuc = await _bakimRiskServisi.RiskHesaplaVeKaydetAsync(id);
            return Json(new
            {
                basarili = true,
                plaka = sonuc.Plaka,
                puan = sonuc.RiskPuani,
                seviye = sonuc.RiskSeviyesi.ToString(),
                oneriler = sonuc.Oneriler
            });
        }
        catch (Exception)
        {
            return Json(new { basarili = false });
        }
    }

    // ---- Arıza Kayıtları ----

    [HttpGet]
    public async Task<IActionResult> ArizaKayitlariPartial(int aracId)
    {
        var kayitlar = await _arizaKaydiServisi.AracIcinGetirAsync(aracId);
        ViewBag.AracId = aracId;
        return PartialView("_ArizaKayitlariPartial", kayitlar);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ArizaEkle(
        [FromForm] int AracId,
        [FromForm] DateTime ArizaTarihi,
        [FromForm] string ArizaTuru,
        [FromForm] string? Aciklama,
        [FromForm] decimal OnarimMaliyeti,
        [FromForm] double ArizaAnindakiKm)
    {
        // Arızanın "o anki" kilometresi, aracın ŞU ANKİ kilometresinden büyük olamaz —
        // km sadece artar, geçmişte yaşanmış bir olay gelecekteki bir km değerinde olamaz.
        var arac = await _aracServisi.AracGetirAsync(AracId);
        if (arac != null && ArizaAnindakiKm > arac.GuncelKm)
        {
            TempData["Hata"] = $"Arıza anındaki km ({ArizaAnindakiKm:N0}), aracın güncel km'sinden ({arac.GuncelKm:N0}) büyük olamaz. " +
                "Ya arıza km'sini kontrol edin, ya da önce aracın güncel km'sini güncelleyin.";
            return RedirectToAction(nameof(Index));
        }

        var kayit = new AracArizaKaydi
        {
            AracId = AracId,
            ArizaTarihi = ArizaTarihi,
            ArizaTuru = ArizaTuru,
            Aciklama = Aciklama ?? string.Empty,
            OnarimMaliyeti = OnarimMaliyeti,
            ArizaAnindakiKm = ArizaAnindakiKm
        };

        await _arizaKaydiServisi.EkleAsync(kayit);
        TempData["Basari"] = "Arıza kaydı eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ArizaSil(int id)
    {
        var silindi = await _arizaKaydiServisi.SilAsync(id);
        return Json(silindi ? "başarılı" : "hata");
    }

    // ---- Bakım Kayıtları ----

    [HttpGet]
    public async Task<IActionResult> BakimKayitlariPartial(int aracId)
    {
        var kayitlar = await _bakimKaydiServisi.AracIcinGetirAsync(aracId);
        ViewBag.AracId = aracId;
        return PartialView("_BakimKayitlariPartial", kayitlar);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BakimEkle(
        [FromForm] int AracId,
        [FromForm] DateTime BakimTarihi,
        [FromForm] string BakimTuru,
        [FromForm] int? SonrakiBakimKm,
        [FromForm] DateTime? SonrakiBakimTarihi,
        [FromForm] decimal Maliyet,
        [FromForm] string? Aciklama)
    {
        var kayit = new BakimKaydi
        {
            AracId = AracId,
            BakimTarihi = BakimTarihi,
            BakimTuru = BakimTuru,
            SonrakiBakimKm = SonrakiBakimKm,
            SonrakiBakimTarihi = SonrakiBakimTarihi,
            Maliyet = Maliyet,
            Aciklama = Aciklama
        };

        await _bakimKaydiServisi.EkleAsync(kayit);
        TempData["Basari"] = "Bakım kaydı eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> BakimSil(int id)
    {
        var silindi = await _bakimKaydiServisi.SilAsync(id);
        return Json(silindi ? "başarılı" : "hata");
    }
}