using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Application.Interfaces;
using AracServisTakipSistemi.Domain.Entities;

namespace AracServisTakipSistemi.Application.Services;

public class PersonelServisi
{
    private readonly IPersonelRepository _repository;
    private readonly IGeocodingServisi _geocodingServisi;

    public PersonelServisi(IPersonelRepository repository, IGeocodingServisi geocodingServisi)
    {
        _repository = repository;
        _geocodingServisi = geocodingServisi;
    }

    public Task<List<Personel>> TumPersonelleriGetirAsync() => _repository.TumunuGetirAsync();

    public Task<List<Personel>> AktifPersonelleriGetirAsync() => _repository.AktifleriGetirAsync();

    public Task<Personel?> PersonelGetirAsync(int id) => _repository.IdIleGetirAsync(id);

    public async Task<(bool GeocodingBasarili, string? Uyari)> PersonelEkleAsync(Personel personel)
    {
        bool geocodingBasarili = true;
        string? uyari = null;

        if (personel.Enlem == null || personel.Boylam == null)
        {
            var sonuc = await _geocodingServisi.AdresteneKoordinatBulAsync(personel.Adres);

            if (sonuc.BasariliMi)
            {
                personel.Enlem = sonuc.Enlem;
                personel.Boylam = sonuc.Boylam;
            }
            else
            {
                geocodingBasarili = false;
                uyari = $"Adres otomatik bulunamadı ({sonuc.HataMesaji}). Lütfen koordinatı elle girin.";
            }
        }

        await _repository.EkleAsync(personel);
        await _repository.KaydetAsync();

        return (geocodingBasarili, uyari);
    }

    public async Task PersonelGuncelleAsync(Personel personel)
    {
        await _repository.GuncelleAsync(personel);
        await _repository.KaydetAsync();
    }

    public async Task IstenCikarAsync(int personelId)
    {
        var personel = await _repository.IdIleGetirAsync(personelId);
        if (personel == null) return;

        personel.AktifMi = false;
        personel.IstenCikisTarihi = DateTime.Now;

        await _repository.GuncelleAsync(personel);
        await _repository.KaydetAsync();
    }
}