using AracServisTakipSistemi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AracServisTakipSistemi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Personel> Personeller => Set<Personel>();
    public DbSet<Arac> Araclar => Set<Arac>();
    public DbSet<AracArizaKaydi> AracArizaKayitlari => Set<AracArizaKaydi>();
    public DbSet<AracAtama> AracAtamalari => Set<AracAtama>();
    public DbSet<Rota> Rotalar => Set<Rota>();
    public DbSet<RotaDuragi> RotaDuraklari => Set<RotaDuragi>();
    public DbSet<SemtReferans> SemtReferanslari => Set<SemtReferans>();
    public DbSet<Vardiya> Vardiyalar => Set<Vardiya>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AracArizaKaydi>()
            .HasOne(a => a.Arac)
            .WithMany(v => v.ArizaKayitlari)
            .HasForeignKey(a => a.AracId);

        modelBuilder.Entity<AracAtama>()
            .HasOne(a => a.Arac)
            .WithMany(v => v.AracAtamalari)
            .HasForeignKey(a => a.AracId);

        modelBuilder.Entity<AracAtama>()
            .HasOne(a => a.Personel)
            .WithMany(p => p.AracAtamalari)
            .HasForeignKey(a => a.PersonelId);

        modelBuilder.Entity<Rota>()
            .HasOne(r => r.Arac)
            .WithMany(v => v.Rotalar)
            .HasForeignKey(r => r.AracId);

        modelBuilder.Entity<RotaDuragi>()
            .HasOne(s => s.Rota)
            .WithMany(r => r.Duraklar)
            .HasForeignKey(s => s.RotaId);

        modelBuilder.Entity<RotaDuragi>()
            .HasOne(s => s.Personel)
            .WithMany(p => p.RotaDuraklari)
            .HasForeignKey(s => s.PersonelId);

        modelBuilder.Entity<Arac>()
            .HasOne(a => a.SoforPersonel)
            .WithMany()
            .HasForeignKey(a => a.SoforPersonelId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Personel>()
            .HasOne(p => p.Vardiya)
            .WithMany(v => v.Personeller)
            .HasForeignKey(p => p.VardiyaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AracArizaKaydi>()
            .Property(m => m.OnarimMaliyeti)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Vardiya>().HasData(
            new Vardiya { Id = 1, VardiyaAdi = "Sabah", BaslangicSaati = new TimeSpan(8, 30, 0), BitisSaati = new TimeSpan(18, 0, 0) }
        );
    }
}
