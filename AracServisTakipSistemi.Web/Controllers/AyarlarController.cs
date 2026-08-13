using AracServisTakipSistemi.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AyarlarController : Controller
{
    private readonly SirketAyarServisi _sirketAyarServisi;

    public AyarlarController(SirketAyarServisi sirketAyarServisi)
    {
        _sirketAyarServisi = sirketAyarServisi;
    }

    public async Task<IActionResult> Index()
    {
        var ayar = await _sirketAyarServisi.GetirAsync();
        return View(ayar);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guncelle(
        [FromForm] string Ad,
        [FromForm] double Enlem,
        [FromForm] double Boylam,
        [FromForm] int GidisVarisTamponDk,
        [FromForm] int DonusKalkisTamponDk,
        [FromForm] double MaksimumBolgeMesafesiKm)
    {
        if (Enlem < -90 || Enlem > 90)
        {
            TempData["Hata"] = "Enlem -90 ile 90 arasında olmalı.";
            return RedirectToAction(nameof(Index));
        }

        if (Boylam < -180 || Boylam > 180)
        {
            TempData["Hata"] = "Boylam -180 ile 180 arasında olmalı.";
            return RedirectToAction(nameof(Index));
        }

        var ayar = await _sirketAyarServisi.GetirAsync();
        ayar.Ad = Ad;
        ayar.Enlem = Enlem;
        ayar.Boylam = Boylam;
        ayar.GidisVarisTamponDk = GidisVarisTamponDk;
        ayar.DonusKalkisTamponDk = DonusKalkisTamponDk;
        ayar.MaksimumBolgeMesafesiKm = MaksimumBolgeMesafesiKm;

        await _sirketAyarServisi.GuncelleAsync(ayar);

        TempData["Basari"] = "Ayarlar güncellendi. Değişikliklerin rota hesaplamasına yansıması için 'Rotaları Yeniden Hesapla'yı çalıştırmanız gerekebilir.";
        return RedirectToAction(nameof(Index));
    }
}