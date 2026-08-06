using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using AracServisTakipSistemi.BLL.Interfaces;
using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Entities.Enums;
using AracServisTakipSistemi.DAL.Data;

namespace AracServisTakipSistemi.DAL.Repositories;

public class PersonelRepository : IPersonelRepository
{
    private readonly ApplicationDbContext _context;

    public PersonelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Personel>> TumunuGetirAsync() =>
        await _context.Personeller.Include(p => p.Vardiya).Include(p => p.Bolge).ToListAsync();

    public async Task<List<Personel>> AktifleriGetirAsync() =>
        await _context.Personeller.Include(p => p.Vardiya).Include(p => p.Bolge)
            .Where(p => p.AktifMi).ToListAsync();

    public async Task<Personel?> IdIleGetirAsync(int id) => await _context.Personeller.FindAsync(id);

    // Kapasite yetersizliği nedeniyle rotaya atanamayıp bekleyen personel
    public async Task<List<Personel>> BeklemedeOlanlariGetirAsync() =>
        await _context.Personeller.Include(p => p.Vardiya).Include(p => p.Bolge)
            .Where(p => p.AktifMi && p.ServisDurumu == ServisDurumu.Beklemede)
            .ToListAsync();

    // Adresi geocode edilemediği (koordinatı bulunamadığı) için bölgesi belirlenemeyen personel
    public async Task<List<Personel>> KoordinatiEksikOlanlariGetirAsync() =>
        await _context.Personeller.Include(p => p.Vardiya).Include(p => p.Bolge)
            .Where(p => p.AktifMi && _context.PersonelAdresleri
                .Any(a => a.PersonelId == p.Id && a.AktifMi && (a.Enlem == null || a.Boylam == null)))
            .ToListAsync();

    public async Task EkleAsync(Personel personel) => await _context.Personeller.AddAsync(personel);

    public Task GuncelleAsync(Personel personel)
    {
        _context.Personeller.Update(personel);
        return Task.CompletedTask;
    }

    public async Task<bool> SilAsync(int id)
    {
        var personel = await _context.Personeller.FindAsync(id);
        if (personel == null) return false;

        var rotaDuraginda = await _context.RotaDuraklari.AnyAsync(rd => rd.PersonelId == id);
        var soforOlarakAtanmis = await _context.Araclar.AnyAsync(a => a.SoforPersonelId == id);
        if (rotaDuraginda || soforOlarakAtanmis) return false;

        _context.Personeller.Remove(personel);
        return true;
    }

    public async Task KaydetAsync() => await _context.SaveChangesAsync();
}