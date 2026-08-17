using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Interfaces;

public interface IAracRepository
{
    Task<List<Arac>> TumunuGetirAsync();
    Task<List<Arac>> AktifleriGetirAsync();
    Task<Arac?> IdIleGetirAsync(int id);

    // Maliyet raporu için — arıza ve bakım kayıtlarını da yükler.
    Task<List<Arac>> TumunuMaliyetVerisiyleGetirAsync();
    Task EkleAsync(Arac arac);
    Task GuncelleAsync(Arac arac);
    Task<bool> SilAsync(int id);
    Task RiskSkoruEkleAsync(RiskSkoru riskSkoru);
    Task KaydetAsync();
}