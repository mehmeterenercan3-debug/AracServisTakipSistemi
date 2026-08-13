using Microsoft.EntityFrameworkCore;
using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.DAL.Data;

namespace AracServisTakipSistemi.DAL.Repositories;

public class SirketAyarRepository : ISirketAyarRepository
{
    private readonly ApplicationDbContext _context;

    public SirketAyarRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Tabloda hep tek satır olmalı — hiç yoksa (ilk çalıştırmada) varsayılan değerlerle oluşturuyoruz
    public async Task<SirketAyar> GetirAsync()
    {
        var ayar = await _context.SirketAyarlari.FirstOrDefaultAsync();
        if (ayar == null)
        {
            ayar = new SirketAyar();
            await _context.SirketAyarlari.AddAsync(ayar);
            await _context.SaveChangesAsync();
        }

        return ayar;
    }

    public Task GuncelleAsync(SirketAyar ayar)
    {
        _context.SirketAyarlari.Update(ayar);
        return Task.CompletedTask;
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}