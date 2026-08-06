using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

[Authorize(Roles = "Admin")]
public class BekleyenPersonelController : Controller
{
    private readonly PersonelServisi _personelServisi;

    public BekleyenPersonelController(PersonelServisi personelServisi)
    {
        _personelServisi = personelServisi;
    }

    public async Task<IActionResult> Index()
    {
        var model = new BekleyenPersonelViewModel
        {
            KapasiteNedeniyleBekleyenler = await _personelServisi.BeklemedeOlanlariGetirAsync(),
            KoordinatiEksikOlanlar = await _personelServisi.KoordinatiEksikOlanlariGetirAsync()
        };

        return View(model);
    }
}