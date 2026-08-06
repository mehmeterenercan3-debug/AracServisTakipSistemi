using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Entities.Enums;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

[Authorize(Roles = "Admin")]
public class PersonelController : Controller
{
    private readonly PersonelServisi _personelServisi;
    private readonly VardiyaServisi _vardiyaServisi;
    private readonly UserManager<ApplicationUser> _userManager;

    public PersonelController(PersonelServisi personelServisi, VardiyaServisi vardiyaServisi, UserManager<ApplicationUser> userManager)
    {
        _personelServisi = personelServisi;
        _vardiyaServisi = vardiyaServisi;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var personeller = await _personelServisi.TumPersonelleriGetirAsync();
        ViewBag.Vardiyalar = await _vardiyaServisi.AktifVardiyalariGetirAsync();

        // Düzenle modal'ını dolduracağımız için her personelin aktif adresini de topluyoruz
        var adresSozlugu = new Dictionary<int, PersonelAdres?>();
        foreach (var p in personeller)
            adresSozlugu[p.Id] = await _personelServisi.AktifAdresiGetirAsync(p.Id);
        ViewBag.Adresler = adresSozlugu;

        // Hangi personelin zaten bir kullanıcı hesabı var, "Hesap Oluştur" butonunu ona göre gösteriyoruz
        var hesapliPersonelIdleri = _userManager.Users
            .Where(u => u.PersonelId != null)
            .Select(u => u.PersonelId!.Value)
            .ToHashSet();
        ViewBag.HesapVarMi = hesapliPersonelIdleri;

        return View(personeller);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PersonelViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Hata"] = "Formda hatalı/eksik alan var, kontrol edin.";
            return RedirectToAction(nameof(Index));
        }

        var personel = new Personel
        {
            Ad = model.Ad,
            Soyad = model.Soyad,
            SicilNo = model.SicilNo,
            PersonelTuru = model.PersonelTuru,
            Cinsiyet = model.Cinsiyet,
            DogumTarihi = model.DogumTarihi,
            Telefon = model.Telefon,
            Eposta = model.Eposta,
            IseGirisTarihi = model.IseGirisTarihi,
            VardiyaId = model.VardiyaId,
            AktifMi = true
        };

        var adres = new PersonelAdres
        {
            AdresTuru = "İkametgah",
            Mahalle = model.Mahalle ?? string.Empty,
            Semt = model.Semt ?? string.Empty,
            IlceAdi = model.IlceAdi ?? string.Empty,
            Sehir = model.Sehir ?? string.Empty,
            Sokak = model.Sokak ?? string.Empty,
            ApartmanNo = model.ApartmanNo ?? string.Empty,
            DisKapiNo = model.DisKapiNo ?? string.Empty
        };

        var (geocodingBasarili, uyari) = await _personelServisi.PersonelEkleAsync(personel, adres);

        if (geocodingBasarili && uyari == null)
        {
            TempData["Basari"] = "Personel eklendi, adresi bulundu ve en yakın bölgeye otomatik atandı.";
        }
        else
        {
            TempData["Hata"] = $"Personel eklendi ama bir sorun var: {uyari}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PersonelViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Hata"] = "Formda hatalı/eksik alan var, kontrol edin.";
            return RedirectToAction(nameof(Index));
        }

        var personel = await _personelServisi.PersonelGetirAsync(model.Id);
        if (personel == null) return NotFound();

        personel.Ad = model.Ad;
        personel.Soyad = model.Soyad;
        personel.SicilNo = model.SicilNo;
        personel.PersonelTuru = model.PersonelTuru;
        personel.Cinsiyet = model.Cinsiyet;
        personel.DogumTarihi = model.DogumTarihi;
        personel.Telefon = model.Telefon;
        personel.Eposta = model.Eposta;
        personel.IseGirisTarihi = model.IseGirisTarihi;
        personel.VardiyaId = model.VardiyaId;

        var yeniAdresVerisi = new PersonelAdres
        {
            AdresTuru = "İkametgah",
            Mahalle = model.Mahalle ?? string.Empty,
            Semt = model.Semt ?? string.Empty,
            IlceAdi = model.IlceAdi ?? string.Empty,
            Sehir = model.Sehir ?? string.Empty,
            Sokak = model.Sokak ?? string.Empty,
            ApartmanNo = model.ApartmanNo ?? string.Empty,
            DisKapiNo = model.DisKapiNo ?? string.Empty
        };

        var (geocodingBasarili, uyari) = await _personelServisi.PersonelVeAdresGuncelleAsync(personel, yeniAdresVerisi);

        if (geocodingBasarili && uyari == null)
            TempData["Basari"] = "Personel ve adres bilgisi güncellendi.";
        else
            TempData["Hata"] = $"Personel güncellendi ama bir sorun var: {uyari}";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> IstenCikar(int id)
    {
        await _personelServisi.IstenCikarAsync(id);
        return Json("başarılı");
    }

    [HttpPost]
    public async Task<IActionResult> Sil(int id)
    {
        var silindiMi = await _personelServisi.PersonelSilAsync(id);
        return Json(silindiMi ? "başarılı" : "hata");
    }

    [HttpPost]
    public async Task<IActionResult> BolgeleriYenidenHesapla()
    {
        var guncellenenSayisi = await _personelServisi.TumBolgeAtamalariniYenidenHesaplaAsync();
        return Json(new { basarili = true, guncellenen = guncellenenSayisi });
    }

    // Personele giriş yapabileceği bir kullanıcı hesabı oluşturur — kullanıcı adı sicil no,
    // varsayılan şifre üretilir, personel türüne göre Şoför/Personel rolü verilir.
    [HttpPost]
    public async Task<IActionResult> KullaniciOlustur(int id)
    {
        var personel = await _personelServisi.PersonelGetirAsync(id);
        if (personel == null)
            return Json(new { basarili = false, mesaj = "Personel bulunamadı." });

        var mevcutKullanici = await _userManager.FindByNameAsync(personel.SicilNo);
        if (mevcutKullanici != null)
            return Json(new { basarili = false, mesaj = "Bu personel için zaten bir kullanıcı hesabı var." });

        var varsayilanSifre = $"Servis{DateTime.Now.Year}!";

        var yeniKullanici = new ApplicationUser
        {
            UserName = personel.SicilNo,
            Email = $"{personel.SicilNo}@aracservis.local",
            PersonelId = personel.Id
        };

        var sonuc = await _userManager.CreateAsync(yeniKullanici, varsayilanSifre);
        if (!sonuc.Succeeded)
        {
            var hatalar = string.Join(", ", sonuc.Errors.Select(e => e.Description));
            return Json(new { basarili = false, mesaj = $"Hesap oluşturulamadı: {hatalar}" });
        }

        var rol = personel.PersonelTuru == PersonelTuru.Sofor ? "Sofor" : "Personel";
        await _userManager.AddToRoleAsync(yeniKullanici, rol);

        return Json(new
        {
            basarili = true,
            kullaniciAdi = personel.SicilNo,
            sifre = varsayilanSifre,
            rol
        });
    }
}