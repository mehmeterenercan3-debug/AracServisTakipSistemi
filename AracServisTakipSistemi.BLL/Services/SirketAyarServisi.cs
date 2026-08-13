using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Services;

public class SirketAyarServisi
{
    private readonly ISirketAyarRepository _repository;

    public SirketAyarServisi(ISirketAyarRepository repository)
    {
        _repository = repository;
    }

    public Task<SirketAyar> GetirAsync() => _repository.GetirAsync();

    public async Task GuncelleAsync(SirketAyar ayar)
    {
        await _repository.GuncelleAsync(ayar);
        await _repository.KaydetAsync();
    }
}