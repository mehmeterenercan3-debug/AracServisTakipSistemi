using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

[Authorize(Roles = "Admin")]
public class VardiyaController : Controller
{
    private readonly VardiyaServisi _vardiyaServisi;

    public VardiyaController(VardiyaServisi vardiyaServisi)
    {
        _vardiyaServisi = vardiyaServisi;
    }

    public async Task<IActionResult> Index()
    {
        var vardiyalar = await _vardiyaServisi.TumVardiyalariGetirAsync();
        return View(vardiyalar);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VardiyaViewModel model)
    {
        if (ModelState.IsValid)
        {
            var vardiya = new Vardiya
            {
                VardiyaAdi = model.VardiyaAdi,
                BaslangicSaati = model.BaslangicSaati,
                BitisSaati = model.BitisSaati,
                AktifMi = model.AktifMi
            };

            await _vardiyaServisi.VardiyaEkleAsync(vardiya);
            TempData["Basari"] = "Vardiya başarıyla eklendi.";
        }
        else
        {
            TempData["Hata"] = "Formda hatalı/eksik alan var, kontrol edin.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(VardiyaViewModel model)
    {
        if (ModelState.IsValid)
        {
            var vardiya = new Vardiya
            {
                Id = model.Id,
                VardiyaAdi = model.VardiyaAdi,
                BaslangicSaati = model.BaslangicSaati,
                BitisSaati = model.BitisSaati,
                AktifMi = model.AktifMi
            };

            await _vardiyaServisi.VardiyaGuncelleAsync(vardiya);
            TempData["Basari"] = "Vardiya başarıyla güncellendi.";
        }
        else
        {
            TempData["Hata"] = "Formda hatalı/eksik alan var, kontrol edin.";
        }

        return RedirectToAction(nameof(Index));
    }

    // Hocanın Categories'te gösterdiği AJAX silme deseniyle birebir aynı
    [HttpPost]
    public async Task<IActionResult> Sil(int id)
    {
        var silindiMi = await _vardiyaServisi.VardiyaSilAsync(id);
        return Json(silindiMi ? "başarılı" : "hata");
    }

    // Aktif/Pasif değiştirme — silmenin alternatifi, ilişkili personel varken kullanılabilir
    [HttpPost]
    public async Task<IActionResult> DurumDegistir(int id)
    {
        var vardiya = await _vardiyaServisi.VardiyaGetirAsync(id);
        if (vardiya == null)
            return Json("hata");

        vardiya.AktifMi = !vardiya.AktifMi;
        await _vardiyaServisi.VardiyaGuncelleAsync(vardiya);

        return Json("başarılı");
    }
}