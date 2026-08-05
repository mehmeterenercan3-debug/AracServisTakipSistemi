using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Services;

public class BakimKaydiServisi
{
    private readonly IBakimKaydiRepository _repository;

    public BakimKaydiServisi(IBakimKaydiRepository repository)
    {
        _repository = repository;
    }

    public Task<List<BakimKaydi>> AracIcinGetirAsync(int aracId) => _repository.AracIcinGetirAsync(aracId);

    public async Task EkleAsync(BakimKaydi kayit)
    {
        await _repository.EkleAsync(kayit);
        await _repository.KaydetAsync();
    }

    public async Task<bool> SilAsync(int id)
    {
        var silindi = await _repository.SilAsync(id);
        if (silindi) await _repository.KaydetAsync();
        return silindi;
    }
}
