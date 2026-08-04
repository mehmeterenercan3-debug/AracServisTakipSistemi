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

public class SemtReferansRepository : ISemtReferansRepository
{
    private readonly ApplicationDbContext _context;

    public SemtReferansRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SemtReferans>> TumunuGetirAsync() =>
        await _context.SemtReferanslari.OrderBy(s => s.SemtAdi).ToListAsync();

    public async Task<SemtReferans?> IdIleGetirAsync(int id) =>
        await _context.SemtReferanslari.FindAsync(id);

    public async Task EkleAsync(SemtReferans semt) => await _context.SemtReferanslari.AddAsync(semt);

    public Task GuncelleAsync(SemtReferans semt)
    {
        _context.SemtReferanslari.Update(semt);
        return Task.CompletedTask;
    }

    public async Task SilAsync(int id)
    {
        var semt = await _context.SemtReferanslari.FindAsync(id);
        if (semt != null) _context.SemtReferanslari.Remove(semt);
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}