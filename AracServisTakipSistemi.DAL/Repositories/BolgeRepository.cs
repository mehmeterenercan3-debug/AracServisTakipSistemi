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

public class BolgeRepository : IBolgeRepository
{
    private readonly ApplicationDbContext _context;

    public BolgeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Bolge>> TumunuGetirAsync() => await _context.Bolgeler.ToListAsync();

    public async Task<List<Bolge>> AktifleriGetirAsync() =>
        await _context.Bolgeler.Where(b => b.AktifMi).ToListAsync();

    public async Task<Bolge?> IdIleGetirAsync(int id) => await _context.Bolgeler.FindAsync(id);

    public async Task EkleAsync(Bolge bolge) => await _context.Bolgeler.AddAsync(bolge);

    public Task GuncelleAsync(Bolge bolge)
    {
        _context.Bolgeler.Update(bolge);
        return Task.CompletedTask;
    }

    public async Task<bool> SilAsync(int id)
    {
        var bolge = await _context.Bolgeler.FindAsync(id);
        if (bolge == null) return false;

        var bagliPersonelVarMi = await _context.Personeller.AnyAsync(p => p.BolgeId == id);
        var bagliRotaVarMi = await _context.RotaBolgeleri.AnyAsync(rb => rb.BolgeId == id);
        if (bagliPersonelVarMi || bagliRotaVarMi) return false;

        _context.Bolgeler.Remove(bolge);
        return true;
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}