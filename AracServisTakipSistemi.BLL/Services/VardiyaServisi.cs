using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Services;

public class VardiyaServisi
{
    private readonly IVardiyaRepository _repository;

    public VardiyaServisi(IVardiyaRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Vardiya>> TumVardiyalariGetirAsync() => _repository.TumunuGetirAsync();

    public Task<List<Vardiya>> AktifVardiyalariGetirAsync() => _repository.AktifleriGetirAsync();

    public Task<Vardiya?> VardiyaGetirAsync(int id) => _repository.IdIleGetirAsync(id);

    public async Task VardiyaEkleAsync(Vardiya vardiya)
    {
        await _repository.EkleAsync(vardiya);
        await _repository.KaydetAsync();
    }

    public async Task VardiyaGuncelleAsync(Vardiya vardiya)
    {
        await _repository.GuncelleAsync(vardiya);
        await _repository.KaydetAsync();
    }

    public async Task<bool> VardiyaSilAsync(int id)
    {
        var silindiMi = await _repository.SilAsync(id);
        if (silindiMi)
            await _repository.KaydetAsync();

        return silindiMi;
    }
}