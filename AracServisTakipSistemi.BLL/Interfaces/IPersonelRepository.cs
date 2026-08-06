using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Interfaces;

public interface IPersonelRepository
{
    Task<List<Personel>> TumunuGetirAsync();
    Task<List<Personel>> AktifleriGetirAsync();
    Task<Personel?> IdIleGetirAsync(int id);
    Task<List<Personel>> BeklemedeOlanlariGetirAsync();
    Task<List<Personel>> KoordinatiEksikOlanlariGetirAsync();
    Task EkleAsync(Personel personel);
    Task GuncelleAsync(Personel personel);
    Task<bool> SilAsync(int id);
    Task KaydetAsync();
}