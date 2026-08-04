using Microsoft.EntityFrameworkCore;
using AracServisTakipSistemi.Domain.Entities;

namespace AracServisTakipSistemi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Personel> Personeller => Set<Personel>();
    public DbSet<PersonelAdres> PersonelAdresleri => Set<PersonelAdres>();
    public DbSet<Bolge> Bolgeler => Set<Bolge>();
    public DbSet<Arac> Araclar => Set<Arac>();
    public DbSet<AracArizaKaydi> AracArizaKayitlari => Set<AracArizaKaydi>();
    public DbSet<BakimKaydi> BakimKayitlari => Set<BakimKaydi>();
    public DbSet<RiskSkoru> RiskSkorlari => Set<RiskSkoru>();
    public DbSet<Rota> Rotalar => Set<Rota>();
    public DbSet<RotaBolge> RotaBolgeleri => Set<RotaBolge>();
    public DbSet<RotaDuragi> RotaDuraklari => Set<RotaDuragi>();
    public DbSet<SemtReferans> SemtReferanslari => Set<SemtReferans>();
    public DbSet<Vardiya> Vardiyalar => Set<Vardiya>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PersonelAdres>()
            .HasOne(a => a.Personel).WithMany(p => p.Adresler).HasForeignKey(a => a.PersonelId);

        modelBuilder.Entity<AracArizaKaydi>()
            .HasOne(a => a.Arac).WithMany(v => v.ArizaKayitlari).HasForeignKey(a => a.AracId);

        modelBuilder.Entity<BakimKaydi>()
            .HasOne(b => b.Arac).WithMany(a => a.BakimKayitlari).HasForeignKey(b => b.AracId);

        modelBuilder.Entity<RiskSkoru>()
            .HasOne(r => r.Arac).WithMany(a => a.RiskSkorlari).HasForeignKey(r => r.AracId);

        modelBuilder.Entity<Rota>()
            .HasOne(r => r.Arac).WithMany(v => v.Rotalar).HasForeignKey(r => r.AracId);

        modelBuilder.Entity<RotaBolge>()
            .HasOne(rb => rb.Rota).WithMany(r => r.Bolgeler).HasForeignKey(rb => rb.RotaId);

        modelBuilder.Entity<RotaBolge>()
            .HasOne(rb => rb.Bolge).WithMany(b => b.RotaBolgeleri).HasForeignKey(rb => rb.BolgeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RotaDuragi>()
            .HasOne(s => s.Rota).WithMany(r => r.Duraklar).HasForeignKey(s => s.RotaId);

        modelBuilder.Entity<RotaDuragi>()
            .HasOne(s => s.Personel).WithMany(p => p.RotaDuraklari).HasForeignKey(s => s.PersonelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Arac>()
            .HasOne(a => a.SoforPersonel).WithMany().HasForeignKey(a => a.SoforPersonelId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Personel>()
            .HasOne(p => p.Vardiya).WithMany(v => v.Personeller).HasForeignKey(p => p.VardiyaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Personel>()
            .HasOne(p => p.Bolge).WithMany(b => b.Personeller).HasForeignKey(p => p.BolgeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AracArizaKaydi>().Property(m => m.OnarimMaliyeti).HasPrecision(18, 2);
        modelBuilder.Entity<BakimKaydi>().Property(b => b.Maliyet).HasPrecision(18, 2);
        modelBuilder.Entity<RiskSkoru>().Property(r => r.SkorDegeri).HasPrecision(5, 2);

        modelBuilder.Entity<Vardiya>().HasData(
            new Vardiya { Id = 1, VardiyaAdi = "Sabah", BaslangicSaati = new TimeSpan(8, 30, 0), BitisSaati = new TimeSpan(18, 0, 0) }
        );
    }
}