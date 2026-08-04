using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.DAL.Data;

namespace AracServisTakipSistemi.DAL.Repositories;

public class AracRepository : IAracRepository
{
    private readonly ApplicationDbContext _context;

    public AracRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Arac>> TumunuGetirAsync() =>
        await _context.Araclar
            .Include(a => a.ArizaKayitlari)
            .Include(a => a.SoforPersonel)
            .ToListAsync();

    public async Task<List<Arac>> AktifleriGetirAsync() =>
        await _context.Araclar
            .Include(a => a.ArizaKayitlari)
            .Include(a => a.SoforPersonel)
            .Where(a => a.AktifMi)
            .ToListAsync();

    public async Task<Arac?> IdIleGetirAsync(int id) =>
        await _context.Araclar
            .Include(a => a.ArizaKayitlari)
            .Include(a => a.SoforPersonel)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task EkleAsync(Arac arac) => await _context.Araclar.AddAsync(arac);

    public Task GuncelleAsync(Arac arac)
    {
        _context.Araclar.Update(arac);
        return Task.CompletedTask;
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}
