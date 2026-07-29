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

public class AracAtamaRepository : IAracAtamaRepository
{
    private readonly ApplicationDbContext _context;

    public AracAtamaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AracAtama?> AktifAtamayiGetirAsync(int personelId) =>
        await _context.AracAtamalari.FirstOrDefaultAsync(a => a.PersonelId == personelId && a.AktifMi);

    public async Task<List<Personel>> AracIcinAktifPersonelleriGetirAsync(int aracId)
    {
        return await _context.AracAtamalari
            .Where(a => a.AracId == aracId && a.AktifMi)
            .Include(a => a.Personel)
            .Select(a => a.Personel!)
            .ToListAsync();
    }

    public async Task EkleAsync(AracAtama atama) => await _context.AracAtamalari.AddAsync(atama);

    public Task GuncelleAsync(AracAtama atama)
    {
        _context.AracAtamalari.Update(atama);
        return Task.CompletedTask;
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}