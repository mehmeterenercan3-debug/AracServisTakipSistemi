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
    Task EkleAsync(Rota rota);
    Task GuncelleAsync(Rota rota);
    Task KaydetAsync();
}