using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Interfaces;

public interface IVardiyaRepository
{
    Task<List<Vardiya>> TumunuGetirAsync();
    Task<List<Vardiya>> AktifleriGetirAsync();
    Task<Vardiya?> IdIleGetirAsync(int id);
    Task EkleAsync(Vardiya vardiya);
    Task GuncelleAsync(Vardiya vardiya);
    Task KaydetAsync();
}