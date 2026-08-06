using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Services;

public class AracServisi
{
    private readonly IAracRepository _repository;

    public AracServisi(IAracRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Arac>> TumAraclariGetirAsync() => _repository.TumunuGetirAsync();

    public Task<List<Arac>> AktifAraclariGetirAsync() => _repository.AktifleriGetirAsync();

    public Task<Arac?> AracGetirAsync(int id) => _repository.IdIleGetirAsync(id);

    // Şoför paneli için — bu personelin şoförü olduğu aracı bul
    public async Task<Arac?> AracSoforIdIleGetirAsync(int personelId)
    {
        var araclar = await _repository.AktifleriGetirAsync();
        return araclar.FirstOrDefault(a => a.SoforPersonelId == personelId);
    }

    public async Task AracEkleAsync(Arac arac)
    {
        await _repository.EkleAsync(arac);
        await _repository.KaydetAsync();
    }

    public async Task AracGuncelleAsync(Arac arac)
    {
        await _repository.GuncelleAsync(arac);
        await _repository.KaydetAsync();
    }

    public async Task<bool> AracSilAsync(int id)
    {
        var silindiMi = await _repository.SilAsync(id);
        if (silindiMi)
            await _repository.KaydetAsync();

        return silindiMi;
    }
}