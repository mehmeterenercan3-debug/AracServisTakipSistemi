using AracServisTakipSistemi.BLL.Services;
using AracServisTakipSistemi.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AracServisTakipSistemi.Web.Controllers;

[Authorize(Roles = "Admin")]
public class MaliyetRaporuController : Controller
{
    private readonly AracServisi _aracServisi;

    public MaliyetRaporuController(AracServisi aracServisi)
    {
        _aracServisi = aracServisi;
    }

    public async Task<IActionResult> Index()
    {
        var araclar = await _aracServisi.TumAraclariMaliyetVerisiyleGetirAsync();

        var buAyBaslangic = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        var buAyArizaMaliyeti = araclar
            .SelectMany(a => a.ArizaKayitlari)
            .Where(a => a.ArizaTarihi >= buAyBaslangic)
            .Sum(a => a.OnarimMaliyeti);

        var buAyBakimMaliyeti = araclar
            .SelectMany(a => a.BakimKayitlari)
            .Where(b => b.BakimTarihi >= buAyBaslangic)
            .Sum(b => b.Maliyet);

        // Her araç için tüm zamanların toplam maliyeti (arıza + bakım) ve km başına maliyet
        var aracMaliyetleri = araclar.Select(a =>
        {
            var arizaToplam = a.ArizaKayitlari.Sum(x => x.OnarimMaliyeti);
            var bakimToplam = a.BakimKayitlari.Sum(x => x.Maliyet);
            var toplam = arizaToplam + bakimToplam;

            return new AracMaliyetOzeti
            {
                AracId = a.Id,
                Plaka = a.Plaka,
                MarkaModel = $"{a.Marka} {a.Model}",
                ToplamMaliyet = toplam,
                GuncelKm = a.GuncelKm,
                // Km sıfırsa bölme hatası olmasın diye 0 gösteriyoruz — henüz kilometre bilgisi girilmemiş demektir
                KmBasinaMaliyet = a.GuncelKm > 0 ? toplam / (decimal)a.GuncelKm : 0,
                ArizaSayisi = a.ArizaKayitlari.Count,
                BakimSayisi = a.BakimKayitlari.Count
            };
        })
        .OrderByDescending(x => x.ToplamMaliyet)
        .ToList();

        var filoToplamKm = araclar.Sum(a => a.GuncelKm);
        var filoToplamMaliyet = aracMaliyetleri.Sum(x => x.ToplamMaliyet);

        var model = new MaliyetRaporuViewModel
        {
            BuAyArizaMaliyeti = buAyArizaMaliyeti,
            BuAyBakimMaliyeti = buAyBakimMaliyeti,
            BuAyToplamMaliyet = buAyArizaMaliyeti + buAyBakimMaliyeti,
            EnMaliyetliUcArac = aracMaliyetleri.Take(3).ToList(),
            TumAraclarMaliyetDetayi = aracMaliyetleri,
            FiloToplamMaliyeti = filoToplamMaliyet,
            FiloOrtalamaKmBasinaMaliyet = filoToplamKm > 0 ? filoToplamMaliyet / (decimal)filoToplamKm : 0
        };

        return View(model);
    }
}