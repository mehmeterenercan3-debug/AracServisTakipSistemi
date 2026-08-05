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

        var model = new AdminDashboardViewModel
        {
            ToplamAktifPersonel = personeller.Count,
            BeklemedeKalanPersonel = personeller.Count(p => p.ServisDurumu == ServisDurumu.Beklemede),
            ToplamAktifArac = araclar.Count(a => a.AktifMi),
            BakimdakiAracSayisi = araclar.Count(a => a.BakimdaMi),
            ToplamBolge = bolgeler.Count
        };

        return View(model);
    }
}