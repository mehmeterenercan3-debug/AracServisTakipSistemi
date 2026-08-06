using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

public class HomeController : Controller
{
    // Artık "hoş geldiniz" sayfası göstermiyor — girişli kullanıcıyı rolüne göre doğru
    // panele yönlendiren bir trafik yönlendirici. Girişsiz kullanıcıyı giriş sayfasına atar.
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToAction("Giris", "Hesap");

        if (User.IsInRole("Admin"))
            return RedirectToAction("Index", "Admin");

        if (User.IsInRole("Sofor") || User.IsInRole("Personel"))
            return RedirectToAction("Index", "Servisim");

        return RedirectToAction("Giris", "Hesap");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}