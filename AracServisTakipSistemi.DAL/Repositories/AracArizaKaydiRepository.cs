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

public class AracArizaKaydiRepository : IAracArizaKaydiRepository
{
    private readonly ApplicationDbContext _context;

    public AracArizaKaydiRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AracArizaKaydi>> AracIcinGetirAsync(int aracId) =>
        await _context.AracArizaKayitlari
            .Where(a => a.AracId == aracId)
            .OrderByDescending(a => a.ArizaTarihi)
            .ToListAsync();

    public async Task EkleAsync(AracArizaKaydi kayit) => await _context.AracArizaKayitlari.AddAsync(kayit);

    public async Task<bool> SilAsync(int id)
    {
        var kayit = await _context.AracArizaKayitlari.FindAsync(id);
        if (kayit == null) return false;
        _context.AracArizaKayitlari.Remove(kayit);
        return true;
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}