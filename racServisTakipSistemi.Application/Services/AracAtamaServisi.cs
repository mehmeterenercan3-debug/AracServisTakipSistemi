using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Application.DTOs;
using AracServisTakipSistemi.Application.Interfaces;
using AracServisTakipSistemi.Domain.Entities;

namespace AracServisTakipSistemi.Application.Services;

public class AracAtamaServisi
{
    private readonly IAracAtamaRepository _aracAtamaRepository;

    public AracAtamaServisi(IAracAtamaRepository aracAtamaRepository)
    {
        _aracAtamaRepository = aracAtamaRepository;
    }

    public async Task AtamalariUygulaAsync(RotaHesaplamaSonucu rotaSonucu)
    {
        foreach (var aracRotasi in rotaSonucu.AracRotalari)
        {
            foreach (var personel in aracRotasi.ZiyaretSirasi)
            {
                var mevcutAtama = await _aracAtamaRepository.AktifAtamayiGetirAsync(personel.Id);

                if (mevcutAtama == null)
                {
                    await _aracAtamaRepository.EkleAsync(new AracAtama
                    {
                        PersonelId = personel.Id,
                        AracId = aracRotasi.AracId,
                        AtamaTarihi = DateTime.Now,
                        AktifMi = true
                    });
                }
                else if (mevcutAtama.AracId != aracRotasi.AracId)
                {
                    mevcutAtama.AktifMi = false;
                    mevcutAtama.AtamaBitisTarihi = DateTime.Now;
                    await _aracAtamaRepository.GuncelleAsync(mevcutAtama);

                    await _aracAtamaRepository.EkleAsync(new AracAtama
                    {
                        PersonelId = personel.Id,
                        AracId = aracRotasi.AracId,
                        AtamaTarihi = DateTime.Now,
                        AktifMi = true
                    });
                }
            }
        }

        foreach (var atanamayanPersonel in rotaSonucu.AtanamayanPersoneller)
        {
            var mevcutAtama = await _aracAtamaRepository.AktifAtamayiGetirAsync(atanamayanPersonel.Id);
            if (mevcutAtama != null)
            {
                mevcutAtama.AktifMi = false;
                mevcutAtama.AtamaBitisTarihi = DateTime.Now;
                await _aracAtamaRepository.GuncelleAsync(mevcutAtama);
            }
        }

        await _aracAtamaRepository.KaydetAsync();
    }

    public async Task<List<Personel>> AracıKullananPersonelleriGetirAsync(int aracId)
    {
        return await _aracAtamaRepository.AracIcinAktifPersonelleriGetirAsync(aracId);
    }

    public async Task AtamayiKapatAsync(int personelId)
    {
        var mevcutAtama = await _aracAtamaRepository.AktifAtamayiGetirAsync(personelId);
        if (mevcutAtama == null) return;

        mevcutAtama.AktifMi = false;
        mevcutAtama.AtamaBitisTarihi = DateTime.Now;

        await _aracAtamaRepository.GuncelleAsync(mevcutAtama);
        await _aracAtamaRepository.KaydetAsync();
    }
}