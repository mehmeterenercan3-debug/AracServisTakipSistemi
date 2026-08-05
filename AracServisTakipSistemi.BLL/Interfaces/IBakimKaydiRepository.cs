using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Interfaces;

public interface IBakimKaydiRepository
{
    Task<List<BakimKaydi>> AracIcinGetirAsync(int aracId);
    Task EkleAsync(BakimKaydi kayit);
    Task<bool> SilAsync(int id);
    Task KaydetAsync();
}