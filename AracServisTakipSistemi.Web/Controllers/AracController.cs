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

    public AracController(AracServisi aracServisi, PersonelServisi personelServisi, BakimRiskServisi bakimRiskServisi)
    {
        _aracServisi = aracServisi;
        _personelServisi = personelServisi;
        _bakimRiskServisi = bakimRiskServisi;
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
}