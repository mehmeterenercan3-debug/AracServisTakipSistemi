using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Domain.Entities;

namespace AracServisTakipSistemi.Application.Interfaces;

public interface IPersonelRepository
{
    Task<List<Personel>> TumunuGetirAsync();
    Task<List<Personel>> AktifleriGetirAsync();
    Task<Personel?> IdIleGetirAsync(int id);
    Task EkleAsync(Personel personel);
    Task GuncelleAsync(Personel personel);
    Task KaydetAsync();
}