using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

[Authorize(Roles = "Admin")]
public class BolgeController : Controller
{
    private readonly BolgeServisi _bolgeServisi;
    private readonly SemtReferansServisi _semtReferansServisi;

    public BolgeController(BolgeServisi bolgeServisi, SemtReferansServisi semtReferansServisi)
    {
        _bolgeServisi = bolgeServisi;
        _semtReferansServisi = semtReferansServisi;
    }

    public async Task<IActionResult> Index()
    {
        var bolgeler = await _bolgeServisi.TumBolgeleriGetirAsync();
        ViewBag.Semtler = await _semtReferansServisi.TumSemtleriGetirAsync();
        return View(bolgeler);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BolgeViewModel model)
    {
        if (ModelState.IsValid)
        {
            var bolge = new Bolge
            {
                BolgeKodu = model.BolgeKodu,
                BolgeAdi = model.BolgeAdi,
                MerkezEnlem = model.MerkezEnlem,
                MerkezBoylam = model.MerkezBoylam,
                MinPersonelEsigi = model.MinPersonelEsigi,
                KapasiteTamponu = model.KapasiteTamponu,
                Aciklama = model.Aciklama,
                AktifMi = model.AktifMi
            };

            await _bolgeServisi.BolgeEkleAsync(bolge);
            TempData["Basari"] = "Bölge başarıyla eklendi.";
        }
        else
        {
            TempData["Hata"] = "Formda hatalı/eksik alan var, kontrol edin.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BolgeViewModel model)
    {
        if (ModelState.IsValid)
        {
            var bolge = new Bolge
            {
                Id = model.Id,
                BolgeKodu = model.BolgeKodu,
                BolgeAdi = model.BolgeAdi,
                MerkezEnlem = model.MerkezEnlem,
                MerkezBoylam = model.MerkezBoylam,
                MinPersonelEsigi = model.MinPersonelEsigi,
                KapasiteTamponu = model.KapasiteTamponu,
                Aciklama = model.Aciklama,
                AktifMi = model.AktifMi
            };

            await _bolgeServisi.BolgeGuncelleAsync(bolge);
            TempData["Basari"] = "Bölge başarıyla güncellendi.";
        }
        else
        {
            TempData["Hata"] = "Formda hatalı/eksik alan var, kontrol edin.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Sil(int id)
    {
        var silindiMi = await _bolgeServisi.BolgeSilAsync(id);
        return Json(silindiMi ? "başarılı" : "hata");
    }

    [HttpPost]
    public async Task<IActionResult> DurumDegistir(int id)
    {
        var bolge = await _bolgeServisi.BolgeGetirAsync(id);
        if (bolge == null)
            return Json("hata");

        bolge.AktifMi = !bolge.AktifMi;
        await _bolgeServisi.BolgeGuncelleAsync(bolge);

        return Json("başarılı");
    }

    // Ekle/Düzenle modallarındaki "Semt seç, koordinat otomatik dolsun" özelliği için
    [HttpGet]
    public async Task<IActionResult> SemtKoordinat(int semtId)
    {
        var semtler = await _semtReferansServisi.TumSemtleriGetirAsync();
        var semt = semtler.FirstOrDefault(s => s.Id == semtId);
        if (semt == null)
            return Json(new { basarili = false });

        return Json(new { basarili = true, enlem = semt.MerkezEnlem, boylam = semt.MerkezBoylam });
    }
}