using AracServisTakipSistemi.Application.Interfaces;
using AracServisTakipSistemi.Domain.Entities;

namespace AracServisTakipSistemi.Application.Services;

public class PersonelServisi
{
    private readonly IPersonelRepository _repository;
    private readonly IPersonelAdresRepository _adresRepository;
    private readonly IGeocodingServisi _geocodingServisi;

    public PersonelServisi(
        IPersonelRepository repository,
        IPersonelAdresRepository adresRepository,
        IGeocodingServisi geocodingServisi)
    {
        _repository = repository;
        _adresRepository = adresRepository;
        _geocodingServisi = geocodingServisi;
    }

    public Task<List<Personel>> TumPersonelleriGetirAsync() => _repository.TumunuGetirAsync();

    public Task<List<Personel>> AktifPersonelleriGetirAsync() => _repository.AktifleriGetirAsync();

    public Task<Personel?> PersonelGetirAsync(int id) => _repository.IdIleGetirAsync(id);

    public async Task<(bool GeocodingBasarili, string? Uyari)> PersonelEkleAsync(Personel personel, PersonelAdres adres)
    {
        await _repository.EkleAsync(personel);
        await _repository.KaydetAsync();

        adres.PersonelId = personel.Id;
        adres.BaslangicTarihi = DateTime.Now;

        bool geocodingBasarili = true;
        string? uyari = null;

        if (adres.Enlem == null || adres.Boylam == null)
        {
            var tamAdres = $"{adres.Sokak} {adres.DisKapiNo}, {adres.Mahalle}, {adres.IlceAdi}, {adres.Sehir}";
            var sonuc = await _geocodingServisi.AdresteneKoordinatBulAsync(tamAdres);

            if (sonuc.BasariliMi)
            {
                adres.Enlem = sonuc.Enlem;
                adres.Boylam = sonuc.Boylam;
                adres.GeocodeTarihi = DateTime.Now;
            }
            else
            {
                geocodingBasarili = false;
                uyari = $"Adres otomatik bulunamadı ({sonuc.HataMesaji}). Lütfen koordinatı elle girin.";
            }
        }

        await _adresRepository.EkleAsync(adres);
        await _adresRepository.KaydetAsync();

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

    public Task<PersonelAdres?> AktifAdresiGetirAsync(int personelId) =>
        _adresRepository.AktifAdresiGetirAsync(personelId);
}