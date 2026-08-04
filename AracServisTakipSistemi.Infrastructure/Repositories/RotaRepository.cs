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

public class RotaRepository : IRotaRepository
{
    private readonly ApplicationDbContext _context;

    public RotaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Rota?> IdIleGetirAsync(int id) =>
        await _context.Rotalar
            .Include(r => r.Arac)
            .Include(r => r.Bolgeler).ThenInclude(rb => rb.Bolge)
            .Include(r => r.Duraklar).ThenInclude(d => d.Personel)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<Rota>> AktifRotalariGetirAsync() =>
        await _context.Rotalar
            .Include(r => r.Arac)
            .Include(r => r.Bolgeler).ThenInclude(rb => rb.Bolge)
            .Include(r => r.Duraklar).ThenInclude(d => d.Personel)
            .Where(r => r.AktifMi)
            .ToListAsync();

    public async Task EkleAsync(Rota rota) => await _context.Rotalar.AddAsync(rota);

    public Task GuncelleAsync(Rota rota)
    {
        _context.Rotalar.Update(rota);
        return Task.CompletedTask;
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}