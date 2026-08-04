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

    public async Task BakimaAlAsync(int aracId)
    {
        var arac = await _repository.IdIleGetirAsync(aracId);
        if (arac == null) return;

        arac.BakimdaMi = true;
        arac.AktifMi = false;

        await _repository.GuncelleAsync(arac);
        await _repository.KaydetAsync();
    }
}