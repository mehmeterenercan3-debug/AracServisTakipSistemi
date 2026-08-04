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

public class VardiyaRepository : IVardiyaRepository
{
    private readonly ApplicationDbContext _context;

    public VardiyaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vardiya>> TumunuGetirAsync() => await _context.Vardiyalar.ToListAsync();

    public async Task<List<Vardiya>> AktifleriGetirAsync() =>
        await _context.Vardiyalar.Where(v => v.AktifMi).ToListAsync();

    public async Task<Vardiya?> IdIleGetirAsync(int id) => await _context.Vardiyalar.FindAsync(id);

    public async Task EkleAsync(Vardiya vardiya) => await _context.Vardiyalar.AddAsync(vardiya);

    public Task GuncelleAsync(Vardiya vardiya)
    {
        _context.Vardiyalar.Update(vardiya);
        return Task.CompletedTask;
    }

    public async Task<bool> SilAsync(int id)
    {
        var vardiya = await _context.Vardiyalar.FindAsync(id);
        if (vardiya == null) return false;

        // Bu vardiyaya bağlı personel varsa silmeyi engelle — veri bütünlüğü için
        var bagliPersonelVarMi = await _context.Personeller.AnyAsync(p => p.VardiyaId == id);
        if (bagliPersonelVarMi) return false;

        _context.Vardiyalar.Remove(vardiya);
        return true;
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}