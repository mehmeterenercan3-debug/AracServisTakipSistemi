using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

public class HesapController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public HesapController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Giris()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Giris(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var sonuc = await _signInManager.PasswordSignInAsync(
            model.KullaniciAdi, model.Sifre, model.BeniHatirla, lockoutOnFailure: false);

        if (!sonuc.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
            return View(model);
        }

        var kullanici = await _userManager.FindByNameAsync(model.KullaniciAdi);
        if (kullanici != null && await _userManager.IsInRoleAsync(kullanici, "Admin"))
            return RedirectToAction("Index", "Admin");

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cikis()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Giris");
    }

    [HttpGet]
    public IActionResult ErisimYok()
    {
        return View();
    }
}