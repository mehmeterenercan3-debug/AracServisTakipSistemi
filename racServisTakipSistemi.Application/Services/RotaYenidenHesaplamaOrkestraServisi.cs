using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Application.DTOs;
using AracServisTakipSistemi.Application.Interfaces;

namespace AracServisTakipSistemi.Application.Services;

public class RotaYenidenHesaplamaOrkestraServisi
{
    private readonly PersonelServisi _personelServisi;
    private readonly AracServisi _aracServisi;
    private readonly RotaHesaplamaServisi _rotaHesaplamaServisi;
    private readonly AracAtamaServisi _aracAtamaServisi;
    private readonly IVardiyaRepository _vardiyaRepository;

    public RotaYenidenHesaplamaOrkestraServisi(
        PersonelServisi personelServisi,
        AracServisi aracServisi,
        RotaHesaplamaServisi rotaHesaplamaServisi,
        AracAtamaServisi aracAtamaServisi,
        IVardiyaRepository vardiyaRepository)
    {
        _personelServisi = personelServisi;
        _aracServisi = aracServisi;
        _rotaHesaplamaServisi = rotaHesaplamaServisi;
        _aracAtamaServisi = aracAtamaServisi;
        _vardiyaRepository = vardiyaRepository;
    }

    public async Task<RotaHesaplamaSonucu> YenidenHesaplaVeUygulaAsync()
    {
        var aktifPersoneller = await _personelServisi.AktifPersonelleriGetirAsync();
        var aktifAraclar = await _aracServisi.AktifAraclariGetirAsync();
        var aktifVardiyalar = await _vardiyaRepository.AktifleriGetirAsync();

        var sonuc =  await _rotaHesaplamaServisi.RotalariHesaplaAsync(aktifPersoneller, aktifAraclar, aktifVardiyalar);

        await _aracAtamaServisi.AtamalariUygulaAsync(sonuc);

        return sonuc;
    }
}