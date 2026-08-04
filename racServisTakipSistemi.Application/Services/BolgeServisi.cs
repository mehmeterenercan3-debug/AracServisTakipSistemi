using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Application.Interfaces;
using AracServisTakipSistemi.Domain.Entities;

namespace AracServisTakipSistemi.Application.Services;

public class BolgeServisi
{
    private readonly IBolgeRepository _repository;

    public BolgeServisi(IBolgeRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Bolge>> TumBolgeleriGetirAsync() => _repository.TumunuGetirAsync();

    public Task<List<Bolge>> AktifBolgeleriGetirAsync() => _repository.AktifleriGetirAsync();

    public async Task BolgeEkleAsync(Bolge bolge)
    {
        await _repository.EkleAsync(bolge);
        await _repository.KaydetAsync();
    }

    public async Task BolgeGuncelleAsync(Bolge bolge)
    {
        await _repository.GuncelleAsync(bolge);
        await _repository.KaydetAsync();
    }
}