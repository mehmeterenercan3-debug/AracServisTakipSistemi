using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Interfaces;

public interface ISirketAyarRepository
{
    Task<SirketAyar> GetirAsync();
    Task GuncelleAsync(SirketAyar ayar);
    Task KaydetAsync();
}