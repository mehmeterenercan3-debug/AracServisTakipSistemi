using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

public class RotaController : Controller
{
    private readonly RotaServisi _rotaServisi;
    private readonly RotaYenidenHesaplamaOrkestraServisi _orkestraServisi;

    public RotaController(RotaServisi rotaServisi, RotaYenidenHesaplamaOrkestraServisi orkestraServisi)
    {
        _rotaServisi = rotaServisi;
        _orkestraServisi = orkestraServisi;
    }

    public async Task<IActionResult> Index()
    {
        var rotalar = await _rotaServisi.AktifRotalariGetirAsync();
        var viewModel = rotalar.Select(r => new RotaViewModel
        {
            Id = r.Id,
            AracId = r.AracId,
            AracPlaka = r.Arac?.Plaka ?? "-",
            Durum = r.Durum.ToString(),
            ToplamMesafeKm = r.ToplamMesafeKm,
            TahminiSureDk = r.TahminiSureDk,
            RotaTarihi = r.RotaTarihi
        }).ToList();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> YenidenHesapla()
    {
        await _orkestraServisi.YenidenHesaplaVeUygulaAsync();
        TempData["Basari"] = "Rotalar başarıyla yeniden hesaplandı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Onayla(int id)
    {
        await _rotaServisi.OnaylaAsync(id);
        return Ok("başarılı");
    }

    [HttpGet]
    public async Task<IActionResult> DetayPartial(int id)
    {
        var rota = await _rotaServisi.RotaGetirAsync(id);
        if (rota == null) return NotFound("Rota bulunamadı.");

        var viewModel = new RotaViewModel
        {
            Id = rota.Id,
            AracId = rota.AracId,
            AracPlaka = rota.Arac?.Plaka ?? "-",
            Durum = rota.Durum.ToString(),
            ToplamMesafeKm = rota.ToplamMesafeKm,
            TahminiSureDk = rota.TahminiSureDk,
            RotaTarihi = rota.RotaTarihi,
            Duraklar = rota.Duraklar?.OrderBy(d => d.SiraNo).Select(d => new RotaDurakViewModel
            {
                SiraNo = d.SiraNo,
                PersonelAdSoyad = d.Personel != null ? $"{d.Personel.Ad} {d.Personel.Soyad}" : $"Personel #{d.PersonelId}",
                VarisSaati = d.TahminiVarisSaati.ToString(@"hh\:mm")
            }).ToList() ?? new List<RotaDurakViewModel>()
        };

        return PartialView("_DetayPartial", viewModel);
    }
}