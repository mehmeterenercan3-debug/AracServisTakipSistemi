using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Domain.Entities;

namespace AracServisTakipSistemi.Application.Interfaces;

public interface IAracAtamaRepository
{
    Task<AracAtama?> AktifAtamayiGetirAsync(int personelId);
    Task<List<Personel>> AracIcinAktifPersonelleriGetirAsync(int aracId);
    Task EkleAsync(AracAtama atama);
    Task GuncelleAsync(AracAtama atama);
    Task KaydetAsync();
}