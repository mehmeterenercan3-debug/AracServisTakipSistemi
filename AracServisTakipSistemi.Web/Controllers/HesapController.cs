using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

public class HesapController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly PersonelServisi _personelServisi;

    public HesapController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, PersonelServisi personelServisi)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _personelServisi = personelServisi;
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
        if (kullanici == null)
        {
            await _signInManager.SignOutAsync();
            ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
            return View(model);
        }

        // Kullanıcı bir Personel kaydına bağlıysa (Şoför/Personel), o personelin hâlâ
        // aktif (işten ayrılmamış) olduğunu doğrula — Identity'nin kendi hesabı bunu bilmiyor.
        if (kullanici.PersonelId != null)
        {
            var personel = await _personelServisi.PersonelGetirAsync(kullanici.PersonelId.Value);
            if (personel == null || !personel.AktifMi)
            {
                await _signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "Bu hesap artık aktif değil. Lütfen yöneticinizle iletişime geçin.");
                return View(model);
            }
        }

        if (await _userManager.IsInRoleAsync(kullanici, "Admin"))
            return RedirectToAction("Index", "Admin");

        if (await _userManager.IsInRoleAsync(kullanici, "Sofor") || await _userManager.IsInRoleAsync(kullanici, "Personel"))
            return RedirectToAction("Index", "Servisim");

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