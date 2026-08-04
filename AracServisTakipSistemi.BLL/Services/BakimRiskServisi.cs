using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.BLL.DTOs;
using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Entities.Enums;

namespace AracServisTakipSistemi.BLL.Services;

public class BakimRiskServisi
{
    private readonly IAracRepository _aracRepository;

    public BakimRiskServisi(IAracRepository aracRepository)
    {
        _aracRepository = aracRepository;
    }

    public BakimRiskSonucu RiskHesapla(Arac arac)
    {
        int puan = 0;
        var oneriler = new List<string>();

        int yas = DateTime.Now.Year - arac.ModelYili;
        if (yas >= 10) puan += 30;
        else if (yas >= 7) puan += 20;
        else if (yas >= 4) puan += 10;

        if (arac.GuncelKm >= 200000) puan += 30;
        else if (arac.GuncelKm >= 120000) puan += 20;
        else if (arac.GuncelKm >= 60000) puan += 10;

        var sonArizalar = arac.ArizaKayitlari
            .Where(a => a.ArizaTarihi >= DateTime.Now.AddMonths(-12))
            .ToList();

        int arizaSayisi = sonArizalar.Count;
        if (arizaSayisi >= 5) puan += 40;
        else if (arizaSayisi >= 3) puan += 25;
        else if (arizaSayisi >= 1) puan += 10;

        puan = Math.Min(puan, 100);

        var seviye = puan switch
        {
            >= 70 => BakimRiskSeviyesi.Kritik,
            >= 45 => BakimRiskSeviyesi.Yuksek,
            >= 20 => BakimRiskSeviyesi.Orta,
            _ => BakimRiskSeviyesi.Dusuk
        };

        if (yas >= 8)
            oneriler.Add("Araç yaşı yüksek — genel kontrol ve olası değişim planlaması önerilir.");
        if (arac.GuncelKm >= 150000)
            oneriler.Add("Yüksek kilometre — periyodik bakım aralığı kısaltılmalı.");
        if (arizaSayisi >= 3)
            oneriler.Add("Son 12 ayda tekrarlayan arızalar var — detaylı teknik muayene önerilir.");

        var enSikArizaTuru = sonArizalar
            .GroupBy(a => a.ArizaTuru)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        if (enSikArizaTuru != null && enSikArizaTuru.Count() >= 2)
            oneriler.Add($"Tekrarlayan arıza türü: '{enSikArizaTuru.Key}' — bu bileşen özellikle incelenmeli.");

        if (arac.MuayeneTarihi.HasValue && arac.MuayeneTarihi.Value < DateTime.Now)
            oneriler.Add("Araç muayene tarihi geçmiş — acilen muayeneye götürülmeli.");

        if (arac.SigortaBitisTarihi.HasValue && arac.SigortaBitisTarihi.Value < DateTime.Now.AddDays(30))
            oneriler.Add("Sigorta bitiş tarihi yaklaşıyor veya geçmiş — yenileme kontrolü yapılmalı.");

        if (oneriler.Count == 0)
            oneriler.Add("Şu an için özel bir risk görülmüyor, standart periyodik bakım yeterli.");

        return new BakimRiskSonucu
        {
            AracId = arac.Id,
            Plaka = arac.Plaka,
            RiskPuani = puan,
            RiskSeviyesi = seviye,
            Oneriler = oneriler
        };
    }

    public async Task<BakimRiskSonucu> RiskHesaplaVeKaydetAsync(Arac arac)
    {
        var sonuc = RiskHesapla(arac);

        arac.RiskSkorlari.Add(new RiskSkoru
        {
            AracId = arac.Id,
            SkorDegeri = sonuc.RiskPuani,
            RiskSeviyesi = sonuc.RiskSeviyesi,
            HesaplamaTarihi = DateTime.Now,
            OnerilenAksiyon = string.Join(" | ", sonuc.Oneriler)
        });

        await _aracRepository.GuncelleAsync(arac);
        await _aracRepository.KaydetAsync();

        return sonuc;
    }
}