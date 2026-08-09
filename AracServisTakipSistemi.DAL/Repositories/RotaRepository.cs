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

public class RotaRepository : IRotaRepository
{
    private readonly ApplicationDbContext _context;

    public RotaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Rota?> IdIleGetirAsync(int id) =>
        await _context.Rotalar
            .Include(r => r.Arac).ThenInclude(a => a!.SoforPersonel)
            .Include(r => r.Bolgeler).ThenInclude(rb => rb.Bolge)
            .Include(r => r.Duraklar).ThenInclude(d => d.Personel)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<Rota>> AktifRotalariGetirAsync() =>
        await _context.Rotalar
            .Include(r => r.Arac).ThenInclude(a => a!.SoforPersonel)
            .Include(r => r.Bolgeler).ThenInclude(rb => rb.Bolge)
            .Include(r => r.Duraklar).ThenInclude(d => d.Personel)
            .Where(r => r.AktifMi)
            .ToListAsync();

    public async Task<List<Rota>> AktifRotalarAracIdIleGetirAsync(int aracId) =>
        await _context.Rotalar
            .Include(r => r.Arac).ThenInclude(a => a!.SoforPersonel)
            .Include(r => r.Duraklar.OrderBy(d => d.SiraNo)).ThenInclude(d => d.Personel)
            .Where(r => r.AracId == aracId && r.AktifMi)
            .ToListAsync();

    public async Task<List<RotaDuragi>> AktifDuraklarPersonelIdIleGetirAsync(int personelId) =>
        await _context.RotaDuraklari
            .Include(d => d.Rota).ThenInclude(r => r!.Arac).ThenInclude(a => a!.SoforPersonel)
            .Where(d => d.PersonelId == personelId && d.Rota!.AktifMi)
            .ToListAsync();

    public async Task EkleAsync(Rota rota) => await _context.Rotalar.AddAsync(rota);

    public Task GuncelleAsync(Rota rota)
    {
        _context.Rotalar.Update(rota);
        return Task.CompletedTask;
    }

    public async Task EskiRotalariSilAsync()
    {
    var rotalar = await _context.Rotalar.ToListAsync();

    var duraklar = await _context.RotaDuraklari.ToListAsync();
    var rotaBolgeleri = await _context.RotaBolgeleri.ToListAsync();

    _context.RotaDuraklari.RemoveRange(duraklar);
    _context.RotaBolgeleri.RemoveRange(rotaBolgeleri);
    _context.Rotalar.RemoveRange(rotalar);

    await _context.SaveChangesAsync();
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}