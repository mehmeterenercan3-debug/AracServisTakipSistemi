using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Interfaces;

public interface IRotaRepository
{
    Task<Rota?> IdIleGetirAsync(int id);
    Task<List<Rota>> AktifRotalariGetirAsync();

    // Bir aracın (Gidiş + Dönüş) tüm aktif rotaları
    Task<List<Rota>> AktifRotalarAracIdIleGetirAsync(int aracId);

    // Bir personelin (Gidiş + Dönüş) tüm aktif durakları
    Task<List<RotaDuragi>> AktifDuraklarPersonelIdIleGetirAsync(int personelId);

    Task EkleAsync(Rota rota);
    Task GuncelleAsync(Rota rota);
    Task KaydetAsync();
    Task EskiRotalariSilAsync();
}