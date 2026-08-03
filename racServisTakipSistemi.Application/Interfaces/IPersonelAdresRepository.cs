using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Domain.Entities;

namespace AracServisTakipSistemi.Application.Interfaces;

public interface IPersonelAdresRepository
{
    Task<PersonelAdres?> AktifAdresiGetirAsync(int personelId);
    Task<Dictionary<int, PersonelAdres>> AktifAdresleriGetirAsync(List<int> personelIdleri);
    Task EkleAsync(PersonelAdres adres);
    Task GuncelleAsync(PersonelAdres adres);
    Task KaydetAsync();
}