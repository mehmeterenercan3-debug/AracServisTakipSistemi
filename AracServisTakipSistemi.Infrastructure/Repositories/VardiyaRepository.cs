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

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}