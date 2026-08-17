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
            .Include(a => a.SoforPersonel)
            .Include(a => a.RiskSkorlari)
            .ToListAsync();

    public async Task<List<Arac>> AktifleriGetirAsync() =>
        await _context.Araclar
            .Include(a => a.SoforPersonel)
            .Include(a => a.RiskSkorlari)
            .Where(a => a.AktifMi)
            .ToListAsync();

    public async Task<Arac?> IdIleGetirAsync(int id) =>
        await _context.Araclar
            .Include(a => a.ArizaKayitlari)
            .Include(a => a.RiskSkorlari)
            .Include(a => a.BakimKayitlari)
            .Include(a => a.SoforPersonel)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<List<Arac>> TumunuMaliyetVerisiyleGetirAsync() =>
        await _context.Araclar
            .Include(a => a.ArizaKayitlari)
            .Include(a => a.BakimKayitlari)
            .ToListAsync();

    public async Task EkleAsync(Arac arac) => await _context.Araclar.AddAsync(arac);

    public Task GuncelleAsync(Arac arac)
    {
        _context.Araclar.Update(arac);
        return Task.CompletedTask;
    }

    public async Task<bool> SilAsync(int id)
    {
        var arac = await _context.Araclar.FindAsync(id);
        if (arac == null) return false;

        var rotasiVarMi = await _context.Rotalar.AnyAsync(r => r.AracId == id);
        if (rotasiVarMi) return false;

        _context.Araclar.Remove(arac);
        return true;
    }

    // Risk skorunu doğrudan ekliyoruz — navigation collection üzerinden değil,
    // EF Core'un değişiklik takibinde belirsizliğe yol açmaması için
    public async Task RiskSkoruEkleAsync(RiskSkoru riskSkoru) => await _context.RiskSkorlari.AddAsync(riskSkoru);

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}
