using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Domain.Entities;

namespace AracServisTakipSistemi.Application.Interfaces;

public interface IBolgeRepository
{
    Task<List<Bolge>> TumunuGetirAsync();
    Task<List<Bolge>> AktifleriGetirAsync();
    Task<Bolge?> IdIleGetirAsync(int id);
    Task EkleAsync(Bolge bolge);
    Task GuncelleAsync(Bolge bolge);
    Task KaydetAsync();
}