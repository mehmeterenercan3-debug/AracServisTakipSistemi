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
    public async Task<IActionResult> Create(AracViewModel model)
    {
        if (ModelState.IsValid)
        {
            var arac = new Arac
            {
                Plaka = model.Plaka,
                Marka = model.Marka,
                Model = model.Model,
                AracTipi = model.AracTipi,
                ModelYili = model.ModelYili,
                GuncelKm = model.GuncelKm,
                SatinAlmaTarihi = model.SatinAlmaTarihi,
                KapasiteSayisi = model.KapasiteSayisi,
                BakimdaMi = model.BakimdaMi,
                SasiNo = model.SasiNo,
                MotorNo = model.MotorNo,
                MuayeneTarihi = model.MuayeneTarihi,
                SigortaBitisTarihi = model.SigortaBitisTarihi,
                SoforPersonelId = model.SoforPersonelId,
                AktifMi = model.AktifMi
            };

            await _aracServisi.AracEkleAsync(arac);
            TempData["Basari"] = "Araç başarıyla eklendi.";
        }
        else
        {
            var hatalar = ModelState
                .Where(kv => kv.Value != null && kv.Value.Errors.Count > 0)
                .Select(kv => $"{kv.Key}: {string.Join(", ", kv.Value!.Errors.Select(e => e.ErrorMessage))}");
            TempData["Hata"] = "Formda hata var — " + string.Join(" | ", hatalar);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AracViewModel model)
    {
        if (ModelState.IsValid)
        {
            var arac = await _aracServisi.AracGetirAsync(model.Id);
            if (arac == null) return NotFound();

            arac.Plaka = model.Plaka;
            arac.Marka = model.Marka;
            arac.Model = model.Model;
            arac.AracTipi = model.AracTipi;
            arac.ModelYili = model.ModelYili;
            arac.GuncelKm = model.GuncelKm;
            arac.SatinAlmaTarihi = model.SatinAlmaTarihi;
            arac.KapasiteSayisi = model.KapasiteSayisi;
            arac.BakimdaMi = model.BakimdaMi;
            arac.SasiNo = model.SasiNo;
            arac.MotorNo = model.MotorNo;
            arac.MuayeneTarihi = model.MuayeneTarihi;
            arac.SigortaBitisTarihi = model.SigortaBitisTarihi;
            arac.SoforPersonelId = model.SoforPersonelId;
            arac.AktifMi = model.AktifMi;

            await _aracServisi.AracGuncelleAsync(arac);
            TempData["Basari"] = "Araç başarıyla güncellendi.";
        }
        else
        {
            var hatalar = ModelState
                .Where(kv => kv.Value != null && kv.Value.Errors.Count > 0)
                .Select(kv => $"{kv.Key}: {string.Join(", ", kv.Value!.Errors.Select(e => e.ErrorMessage))}");
            TempData["Hata"] = "Formda hata var — " + string.Join(" | ", hatalar);
        }

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