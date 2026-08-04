using AracServisTakipSistemi.BLL.DTOs;
using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Entities.Enums;

namespace AracServisTakipSistemi.BLL.Services;

public class RotaYenidenHesaplamaOrkestraServisi
{
    private readonly PersonelServisi _personelServisi;
    private readonly AracServisi _aracServisi;
    private readonly BolgeServisi _bolgeServisi;
    private readonly IVardiyaRepository _vardiyaRepository;
    private readonly IPersonelAdresRepository _personelAdresRepository;
    private readonly RotaHesaplamaServisi _rotaHesaplamaServisi;
    private readonly IRotaRepository _rotaRepository;

    public RotaYenidenHesaplamaOrkestraServisi(
        PersonelServisi personelServisi,
        AracServisi aracServisi,
        BolgeServisi bolgeServisi,
        IVardiyaRepository vardiyaRepository,
        IPersonelAdresRepository personelAdresRepository,
        RotaHesaplamaServisi rotaHesaplamaServisi,
        IRotaRepository rotaRepository)
    {
        _personelServisi = personelServisi;
        _aracServisi = aracServisi;
        _bolgeServisi = bolgeServisi;
        _vardiyaRepository = vardiyaRepository;
        _personelAdresRepository = personelAdresRepository;
        _rotaHesaplamaServisi = rotaHesaplamaServisi;
        _rotaRepository = rotaRepository;
    }

    public async Task<RotaHesaplamaSonucu> YenidenHesaplaVeUygulaAsync()
    {
        var aktifPersoneller = await _personelServisi.AktifPersonelleriGetirAsync();
        var aktifAraclar = await _aracServisi.AktifAraclariGetirAsync();
        var aktifBolgeler = await _bolgeServisi.AktifBolgeleriGetirAsync();
        var aktifVardiyalar = await _vardiyaRepository.AktifleriGetirAsync();

        var ilgiliIdler = aktifPersoneller.Select(p => p.Id)
            .Concat(aktifAraclar.Where(a => a.SoforPersonelId.HasValue).Select(a => a.SoforPersonelId!.Value))
            .Distinct().ToList();
        var adresSozlugu = await _personelAdresRepository.AktifAdresleriGetirAsync(ilgiliIdler);

        var sonuc = _rotaHesaplamaServisi.RotalariHesapla(aktifPersoneller, aktifAraclar, aktifBolgeler, aktifVardiyalar, adresSozlugu);

        // Eski aktif rotaları pasife çek
        var eskiRotalar = await _rotaRepository.AktifRotalariGetirAsync();
        foreach (var eski in eskiRotalar)
        {
            eski.AktifMi = false;
            await _rotaRepository.GuncelleAsync(eski);
        }

        // Yeni rotaları kaydet — onay bekliyor durumunda
        foreach (var rotaSonucu in sonuc.Rotalar)
        {
            var yeniRota = new Rota
            {
                AracId = rotaSonucu.AracId,
                Durum = RotaDurumu.OnayBekliyor,
                RotaTarihi = DateTime.Today,
                ToplamMesafeKm = 0,
                TahminiSureDk = rotaSonucu.TahminiToplamSureDk,
                AktifMi = true
            };

            foreach (var bolgeId in rotaSonucu.BolgeIdleri)
                yeniRota.Bolgeler.Add(new RotaBolge { BolgeId = bolgeId });

            for (int i = 0; i < rotaSonucu.ZiyaretSirasi.Count; i++)
            {
                yeniRota.Duraklar.Add(new RotaDuragi
                {
                    PersonelId = rotaSonucu.ZiyaretSirasi[i].Id,
                    SiraNo = i + 1,
                    TahminiVarisSaati = rotaSonucu.GidisKalkisSaati
                });
            }

            await _rotaRepository.EkleAsync(yeniRota);
        }

        await _rotaRepository.KaydetAsync();

        // Personel durumlarını (ServisDurumu, BolgeId değişmediyse dokunmuyoruz) güncelle
        foreach (var p in aktifPersoneller)
            await _personelServisi.PersonelGuncelleAsync(p);

        return sonuc;
    }
}