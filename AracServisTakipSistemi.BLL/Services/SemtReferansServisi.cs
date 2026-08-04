using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Services;

public class SemtReferansServisi
{
    private readonly ISemtReferansRepository _repository;

    public SemtReferansServisi(ISemtReferansRepository repository)
    {
        _repository = repository;
    }

    public Task<List<SemtReferans>> TumSemtleriGetirAsync() => _repository.TumunuGetirAsync();

    public async Task SemtEkleAsync(string semtAdi, double enlem, double boylam)
    {
        await _repository.EkleAsync(new SemtReferans
        {
            SemtAdi = semtAdi,
            MerkezEnlem = enlem,
            MerkezBoylam = boylam
        });
        await _repository.KaydetAsync();
    }

    public async Task SemtGuncelleAsync(int id, string semtAdi, double enlem, double boylam)
    {
        var semt = await _repository.IdIleGetirAsync(id);
        if (semt == null) return;

        semt.SemtAdi = semtAdi;
        semt.MerkezEnlem = enlem;
        semt.MerkezBoylam = boylam;

        await _repository.GuncelleAsync(semt);
        await _repository.KaydetAsync();
    }

    public async Task SemtSilAsync(int id)
    {
        await _repository.SilAsync(id);
        await _repository.KaydetAsync();
    }
}