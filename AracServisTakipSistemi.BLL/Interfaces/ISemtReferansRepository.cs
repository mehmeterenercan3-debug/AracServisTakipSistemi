using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.Interfaces;

public interface ISemtReferansRepository
{
    Task<List<SemtReferans>> TumunuGetirAsync();
    Task<SemtReferans?> IdIleGetirAsync(int id);
    Task EkleAsync(SemtReferans semt);
    Task GuncelleAsync(SemtReferans semt);
    Task SilAsync(int id);
    Task KaydetAsync();
}