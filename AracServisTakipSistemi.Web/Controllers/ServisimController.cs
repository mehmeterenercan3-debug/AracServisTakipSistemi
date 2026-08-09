using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Entities.Enums;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

[Authorize(Roles = "Sofor,Personel")]
public class ServisimController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AracServisi _aracServisi;
    private readonly RotaServisi _rotaServisi;
    private readonly PersonelServisi _personelServisi;

    public ServisimController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AracServisi aracServisi,
        RotaServisi rotaServisi,
        PersonelServisi personelServisi)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _aracServisi = aracServisi;
        _rotaServisi = rotaServisi;
        _personelServisi = personelServisi;
    }

    public async Task<IActionResult> Index()
    {
        var kullanici = await _userManager.GetUserAsync(User);
        var roller = await _userManager.GetRolesAsync(kullanici!);

        if (kullanici?.PersonelId == null)
        {
            return View(new ServisimViewModel
            {
                KayitBulunduMu = false,
                BilgiMesaji = "Hesabınız henüz bir personel kaydına bağlanmamış. Lütfen yöneticinizle iletişime geçin."
            });
        }

        var personelId = kullanici.PersonelId.Value;

        if (roller.Contains("Sofor"))
            return View(await SoforGorunumuOlusturAsync(personelId));

        return View(await PersonelGorunumuOlusturAsync(personelId));
    }

    private async Task<ServisimViewModel> SoforGorunumuOlusturAsync(int personelId)
    {
        var arac = await _aracServisi.AracSoforIdIleGetirAsync(personelId);
        if (arac == null)
        {
            return new ServisimViewModel
            {
                KayitBulunduMu = false,
                BilgiMesaji = "Şu an size atanmış bir servis aracı bulunmuyor."
            };
        }

        var rotalar = await _rotaServisi.AktifRotalarAracIdIleGetirAsync(arac.Id);
        if (rotalar.Count == 0)
        {
            return new ServisimViewModel
            {
                KayitBulunduMu = false,
                BilgiMesaji = $"'{arac.Plaka}' plakalı aracınız için henüz hesaplanmış bir rota yok."
            };
        }

        var model = new ServisimViewModel
        {
            KayitBulunduMu = true,
            Rol = "Sofor",
            AracPlaka = arac.Plaka
        };

        var gidisRota = rotalar.FirstOrDefault(r => r.Yon == RotaYonu.Gidis);
        var donusRota = rotalar.FirstOrDefault(r => r.Yon == RotaYonu.Donus);

        if (gidisRota != null) model.Gidis = await RotaYiYonBilgisineCevirAsync(gidisRota);
        if (donusRota != null) model.Donus = await RotaYiYonBilgisineCevirAsync(donusRota);

        return model;
    }

    private async Task<ServisimYonBilgisi> RotaYiYonBilgisineCevirAsync(Rota rota)
    {
        var yonBilgisi = new ServisimYonBilgisi
        {
            RotaDurumu = rota.Durum.ToString(),
            ToplamMesafeKm = rota.ToplamMesafeKm,
            TahminiSureDk = rota.TahminiSureDk
        };

        foreach (var durak in rota.Duraklar.OrderBy(d => d.SiraNo))
        {
            var adresMetni = await AcikAdresGetirAsync(durak.PersonelId);

            yonBilgisi.TumDuraklar.Add(new ServisimDurakViewModel
            {
                SiraNo = durak.SiraNo,
                PersonelAdSoyad = durak.Personel != null ? $"{durak.Personel.Ad} {durak.Personel.Soyad}" : $"Personel #{durak.PersonelId}",
                PersonelTelefon = durak.Personel?.Telefon ?? "-",
                VarisSaati = durak.TahminiVarisSaati.ToString(@"hh\:mm"),
                Adres = adresMetni
            });
        }

        return yonBilgisi;
    }

    // Şoförün gerçekten evi bulabilmesi için okunabilir, açık adres metni üretir
    private async Task<string> AcikAdresGetirAsync(int personelId)
    {
        var adres = await _personelServisi.AktifAdresiGetirAsync(personelId);
        if (adres == null) return "Adres bilgisi yok";

        var parcalar = new List<string>();
        if (!string.IsNullOrWhiteSpace(adres.Sokak)) parcalar.Add(adres.Sokak);
        if (!string.IsNullOrWhiteSpace(adres.DisKapiNo)) parcalar.Add($"No:{adres.DisKapiNo}");
        if (!string.IsNullOrWhiteSpace(adres.Mahalle)) parcalar.Add(adres.Mahalle + " Mah.");
        if (!string.IsNullOrWhiteSpace(adres.IlceAdi)) parcalar.Add(adres.IlceAdi);
        if (!string.IsNullOrWhiteSpace(adres.Sehir)) parcalar.Add(adres.Sehir);

        return parcalar.Count > 0 ? string.Join(", ", parcalar) : "Adres bilgisi yok";
    }

    private async Task<ServisimViewModel> PersonelGorunumuOlusturAsync(int personelId)
    {
        var duraklar = await _rotaServisi.AktifDuraklarPersonelIdIleGetirAsync(personelId);
        if (duraklar.Count == 0)
        {
            return new ServisimViewModel
            {
                KayitBulunduMu = false,
                BilgiMesaji = "Şu an için size atanmış bir servis/rota bulunmuyor."
            };
        }

        var gidisDurak = duraklar.FirstOrDefault(d => d.Rota?.Yon == RotaYonu.Gidis);
        var donusDurak = duraklar.FirstOrDefault(d => d.Rota?.Yon == RotaYonu.Donus);
        var ornekArac = (gidisDurak ?? donusDurak)?.Rota?.Arac;

        var model = new ServisimViewModel
        {
            KayitBulunduMu = true,
            Rol = "Personel",
            AracPlaka = ornekArac?.Plaka ?? "-",
            SoforAdSoyad = ornekArac?.SoforPersonel != null ? $"{ornekArac.SoforPersonel.Ad} {ornekArac.SoforPersonel.Soyad}" : "-",
            SoforTelefon = ornekArac?.SoforPersonel?.Telefon ?? "-"
        };

        if (gidisDurak != null)
        {
            model.Gidis = new ServisimYonBilgisi
            {
                RotaDurumu = gidisDurak.Rota!.Durum.ToString(),
                KendiSiraNo = gidisDurak.SiraNo,
                KendiVarisSaati = gidisDurak.TahminiVarisSaati.ToString(@"hh\:mm")
            };
        }

        if (donusDurak != null)
        {
            model.Donus = new ServisimYonBilgisi
            {
                RotaDurumu = donusDurak.Rota!.Durum.ToString(),
                KendiSiraNo = donusDurak.SiraNo,
                KendiVarisSaati = donusDurak.TahminiVarisSaati.ToString(@"hh\:mm")
            };
        }

        return model;
    }

    [HttpGet]
    public IActionResult SifreDegistir()
    {
        return View(new SifreDegistirViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SifreDegistir(SifreDegistirViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var kullanici = await _userManager.GetUserAsync(User);
        if (kullanici == null) return RedirectToAction("Giris", "Hesap");

        var sonuc = await _userManager.ChangePasswordAsync(kullanici, model.MevcutSifre, model.YeniSifre);
        if (!sonuc.Succeeded)
        {
            foreach (var hata in sonuc.Errors)
                ModelState.AddModelError(string.Empty, hata.Description);
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(kullanici);

        TempData["Basari"] = "Şifreniz başarıyla değiştirildi.";
        return RedirectToAction(nameof(Index));
    }
}
