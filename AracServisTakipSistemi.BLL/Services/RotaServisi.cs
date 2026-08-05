using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Entities.Enums;

namespace AracServisTakipSistemi.BLL.Services;

public class RotaServisi
{
    private readonly IRotaRepository _repository;

    public RotaServisi(IRotaRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Rota>> AktifRotalariGetirAsync() => _repository.AktifRotalariGetirAsync();

    public Task<Rota?> RotaGetirAsync(int id) => _repository.IdIleGetirAsync(id);

    public async Task OnaylaAsync(int id)
    {
        var rota = await _repository.IdIleGetirAsync(id);
        if (rota == null) return;

        rota.Durum = RotaDurumu.Onaylandi;
        await _repository.GuncelleAsync(rota);
        await _repository.KaydetAsync();
    }
}
