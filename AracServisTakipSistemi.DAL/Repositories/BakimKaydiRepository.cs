using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.DAL.Data;
using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.DAL.Repositories;

public class BakimKaydiRepository : IBakimKaydiRepository
{
    private readonly ApplicationDbContext _context;

    public BakimKaydiRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BakimKaydi>> AracIcinGetirAsync(int aracId) =>
        await _context.BakimKayitlari
            .Where(b => b.AracId == aracId)
            .OrderByDescending(b => b.BakimTarihi)
            .ToListAsync();

    public async Task EkleAsync(BakimKaydi kayit) => await _context.BakimKayitlari.AddAsync(kayit);

    public async Task<bool> SilAsync(int id)
    {
        var kayit = await _context.BakimKayitlari.FindAsync(id);
        if (kayit == null) return false;
        _context.BakimKayitlari.Remove(kayit);
        return true;
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}