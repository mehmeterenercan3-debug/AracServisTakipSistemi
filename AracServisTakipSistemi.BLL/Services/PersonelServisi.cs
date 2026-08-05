using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Services;

public class PersonelServisi
{
    private readonly IPersonelRepository _repository;
    private readonly IPersonelAdresRepository _adresRepository;
    private readonly IGeocodingServisi _geocodingServisi;
    private readonly IBolgeRepository _bolgeRepository;
    private readonly IMesafeHesaplayici _mesafeHesaplayici;

    public PersonelServisi(
        IPersonelRepository repository,
        IPersonelAdresRepository adresRepository,
        IGeocodingServisi geocodingServisi,
        IBolgeRepository bolgeRepository,
        IMesafeHesaplayici mesafeHesaplayici)
    {
        _repository = repository;
        _adresRepository = adresRepository;
        _geocodingServisi = geocodingServisi;
        _bolgeRepository = bolgeRepository;
        _mesafeHesaplayici = mesafeHesaplayici;
    }

    public Task<List<Personel>> TumPersonelleriGetirAsync() => _repository.TumunuGetirAsync();

    public Task<List<Personel>> AktifPersonelleriGetirAsync() => _repository.AktifleriGetirAsync();

    public Task<Personel?> PersonelGetirAsync(int id) => _repository.IdIleGetirAsync(id);

    // 1. adımdan kalan basit ekleme — adres almayan senaryolarda hâlâ kullanılabilir
    public async Task PersonelEkleBasitAsync(Personel personel)
    {
        await _repository.EkleAsync(personel);
        await _repository.KaydetAsync();
    }

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
                adres.GeocodeKaynagi = "Otomatik";
                adres.GeocodeBasariliMi = true;
            }
            else
            {
                geocodingBasarili = false;
                adres.GeocodeBasariliMi = false;
                uyari = $"Adres otomatik bulunamadı ({sonuc.HataMesaji}). Bölge ataması yapılamadı, koordinatı elle girip tekrar deneyin.";
            }
        }
        else
        {
            adres.GeocodeKaynagi = "Manuel";
            adres.GeocodeBasariliMi = true;
            adres.GeocodeTarihi = DateTime.Now;
        }

        await _adresRepository.EkleAsync(adres);
        await _adresRepository.KaydetAsync();

        // Geocoding başarılıysa, en yakın bölgeyi otomatik ata
        if (geocodingBasarili && adres.Enlem.HasValue && adres.Boylam.HasValue)
        {
            var atandiMi = await EnYakinBolgeyeAtaAsync(personel, adres.Enlem.Value, adres.Boylam.Value);
            if (!atandiMi)
                uyari = "Koordinat bulundu ama tanımlı bir bölge merkezi olmadığı için otomatik bölge ataması yapılamadı.";
        }

        return (geocodingBasarili, uyari);
    }

    private async Task<bool> EnYakinBolgeyeAtaAsync(Personel personel, double enlem, double boylam)
    {
        var bolgeler = await _bolgeRepository.AktifleriGetirAsync();
        var koordinatiOlanBolgeler = bolgeler.Where(b => b.MerkezEnlem.HasValue && b.MerkezBoylam.HasValue).ToList();

        if (koordinatiOlanBolgeler.Count == 0) return false;

        var enYakinBolge = koordinatiOlanBolgeler
            .OrderBy(b => _mesafeHesaplayici.MesafeHesaplaKm(enlem, boylam, b.MerkezEnlem!.Value, b.MerkezBoylam!.Value))
            .First();

        personel.BolgeId = enYakinBolge.Id;
        await _repository.GuncelleAsync(personel);
        await _repository.KaydetAsync();

        return true;
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

    public async Task<bool> PersonelSilAsync(int id)
    {
        var silindiMi = await _repository.SilAsync(id);
        if (silindiMi)
            await _repository.KaydetAsync();

        return silindiMi;
    }

    public Task<PersonelAdres?> AktifAdresiGetirAsync(int personelId) =>
        _adresRepository.AktifAdresiGetirAsync(personelId);
}