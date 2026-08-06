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

        var (geocodingBasarili, uyari) = await AdresiGeocodeEtAsync(adres);

        await _adresRepository.EkleAsync(adres);
        await _adresRepository.KaydetAsync();

        if (geocodingBasarili && adres.Enlem.HasValue && adres.Boylam.HasValue)
        {
            var atandiMi = await EnYakinBolgeyeAtaAsync(personel, adres.Enlem.Value, adres.Boylam.Value);
            if (!atandiMi)
                uyari = "Koordinat bulundu ama tanımlı bir bölge merkezi olmadığı için otomatik bölge ataması yapılamadı.";
        }

        return (geocodingBasarili, uyari);
    }

    // Personel düzenlenirken adres bilgisi de değiştiyse: adresi güncelle, yeniden geocode et, bölgeyi yeniden ata
    public async Task<(bool GeocodingBasarili, string? Uyari)> PersonelVeAdresGuncelleAsync(Personel personel, PersonelAdres yeniAdresVerisi)
    {
        await _repository.GuncelleAsync(personel);
        await _repository.KaydetAsync();

        var mevcutAdres = await _adresRepository.AktifAdresiGetirAsync(personel.Id);
        if (mevcutAdres == null)
        {
            yeniAdresVerisi.PersonelId = personel.Id;
            yeniAdresVerisi.BaslangicTarihi = DateTime.Now;

            var (basariliYeni, uyariYeni) = await AdresiGeocodeEtAsync(yeniAdresVerisi);
            await _adresRepository.EkleAsync(yeniAdresVerisi);
            await _adresRepository.KaydetAsync();

            if (basariliYeni && yeniAdresVerisi.Enlem.HasValue && yeniAdresVerisi.Boylam.HasValue)
                await EnYakinBolgeyeAtaAsync(personel, yeniAdresVerisi.Enlem.Value, yeniAdresVerisi.Boylam.Value);

            return (basariliYeni, uyariYeni);
        }

        bool adresDegisti =
            mevcutAdres.Mahalle != yeniAdresVerisi.Mahalle ||
            mevcutAdres.Semt != yeniAdresVerisi.Semt ||
            mevcutAdres.IlceAdi != yeniAdresVerisi.IlceAdi ||
            mevcutAdres.Sehir != yeniAdresVerisi.Sehir ||
            mevcutAdres.Sokak != yeniAdresVerisi.Sokak ||
            mevcutAdres.ApartmanNo != yeniAdresVerisi.ApartmanNo ||
            mevcutAdres.DisKapiNo != yeniAdresVerisi.DisKapiNo;

        if (!adresDegisti)
            return (true, null);

        mevcutAdres.Mahalle = yeniAdresVerisi.Mahalle;
        mevcutAdres.Semt = yeniAdresVerisi.Semt;
        mevcutAdres.IlceAdi = yeniAdresVerisi.IlceAdi;
        mevcutAdres.Sehir = yeniAdresVerisi.Sehir;
        mevcutAdres.Sokak = yeniAdresVerisi.Sokak;
        mevcutAdres.ApartmanNo = yeniAdresVerisi.ApartmanNo;
        mevcutAdres.DisKapiNo = yeniAdresVerisi.DisKapiNo;
        mevcutAdres.Enlem = null;
        mevcutAdres.Boylam = null;

        var (geocodingBasarili, uyari) = await AdresiGeocodeEtAsync(mevcutAdres);

        await _adresRepository.GuncelleAsync(mevcutAdres);
        await _adresRepository.KaydetAsync();

        if (geocodingBasarili && mevcutAdres.Enlem.HasValue && mevcutAdres.Boylam.HasValue)
        {
            var atandiMi = await EnYakinBolgeyeAtaAsync(personel, mevcutAdres.Enlem.Value, mevcutAdres.Boylam.Value);
            if (!atandiMi)
                uyari = "Koordinat bulundu ama tanımlı bir bölge merkezi olmadığı için otomatik bölge ataması yapılamadı.";
        }

        return (geocodingBasarili, uyari);
    }

    private async Task<(bool GeocodingBasarili, string? Uyari)> AdresiGeocodeEtAsync(PersonelAdres adres)
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
            return (true, null);
        }

        adres.GeocodeBasariliMi = false;
        return (false, $"Adres otomatik bulunamadı ({sonuc.HataMesaji}). Bölge ataması yapılamadı.");
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

    public async Task<int> TumBolgeAtamalariniYenidenHesaplaAsync()
    {
        var personeller = await _repository.AktifleriGetirAsync();
        int guncellenen = 0;

        foreach (var personel in personeller)
        {
            var adres = await _adresRepository.AktifAdresiGetirAsync(personel.Id);
            if (adres == null || adres.Enlem == null || adres.Boylam == null) continue;

            var eskiBolgeId = personel.BolgeId;
            var atandiMi = await EnYakinBolgeyeAtaAsync(personel, adres.Enlem.Value, adres.Boylam.Value);

            if (atandiMi && personel.BolgeId != eskiBolgeId)
                guncellenen++;
        }

        return guncellenen;
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

    public Task<List<Personel>> BeklemedeOlanlariGetirAsync() => _repository.BeklemedeOlanlariGetirAsync();

    public Task<List<Personel>> KoordinatiEksikOlanlariGetirAsync() => _repository.KoordinatiEksikOlanlariGetirAsync();

    public Task<PersonelAdres?> AktifAdresiGetirAsync(int personelId) =>
        _adresRepository.AktifAdresiGetirAsync(personelId);
}