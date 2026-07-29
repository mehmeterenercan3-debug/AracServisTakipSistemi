using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Domain.Entities;

namespace AracServisTakipSistemi.Application.Interfaces;

public interface IAracRepository
{
    Task<List<Arac>> TumunuGetirAsync();
    Task<List<Arac>> AktifleriGetirAsync();
    Task<Arac?> IdIleGetirAsync(int id);
    Task EkleAsync(Arac arac);
    Task GuncelleAsync(Arac arac);
    Task KaydetAsync();
}