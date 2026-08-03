using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using AracServisTakipSistemi.Application.Interfaces;
using AracServisTakipSistemi.Domain.Entities;
using AracServisTakipSistemi.Infrastructure.Data;

namespace AracServisTakipSistemi.Infrastructure.Repositories;

public class PersonelAdresRepository : IPersonelAdresRepository
{
    private readonly ApplicationDbContext _context;

    public PersonelAdresRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PersonelAdres?> AktifAdresiGetirAsync(int personelId) =>
        await _context.PersonelAdresleri
            .FirstOrDefaultAsync(a => a.PersonelId == personelId && a.AktifMi);

    public async Task<Dictionary<int, PersonelAdres>> AktifAdresleriGetirAsync(List<int> personelIdleri)
    {
        var adresler = await _context.PersonelAdresleri
            .Where(a => personelIdleri.Contains(a.PersonelId) && a.AktifMi)
            .ToListAsync();

        return adresler.ToDictionary(a => a.PersonelId, a => a);
    }

    public async Task EkleAsync(PersonelAdres adres) => await _context.PersonelAdresleri.AddAsync(adres);

    public Task GuncelleAsync(PersonelAdres adres)
    {
        _context.PersonelAdresleri.Update(adres);
        return Task.CompletedTask;
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}