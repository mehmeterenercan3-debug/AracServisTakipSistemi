using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Interfaces;

public interface IAracArizaKaydiRepository
{
    Task<List<AracArizaKaydi>> AracIcinGetirAsync(int aracId);
    Task EkleAsync(AracArizaKaydi kayit);
    Task<bool> SilAsync(int id);
    Task KaydetAsync();
}