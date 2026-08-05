using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

[Authorize(Roles = "Admin")]
public class PersonelController : Controller
{
    private readonly PersonelServisi _personelServisi;
    private readonly VardiyaServisi _vardiyaServisi;

    public PersonelController(PersonelServisi personelServisi, VardiyaServisi vardiyaServisi)
    {
        _personelServisi = personelServisi;
        _vardiyaServisi = vardiyaServisi;
    }

    public async Task<IActionResult> Index()
    {
        var personeller = await _personelServisi.TumPersonelleriGetirAsync();
        ViewBag.Vardiyalar = await _vardiyaServisi.AktifVardiyalariGetirAsync();
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
        if (ModelState.IsValid)
        {
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

            await _personelServisi.PersonelGuncelleAsync(personel);
            TempData["Basari"] = "Personel başarıyla güncellendi.";
        }
        else
        {
            TempData["Hata"] = "Formda hatalı/eksik alan var, kontrol edin.";
        }

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
}