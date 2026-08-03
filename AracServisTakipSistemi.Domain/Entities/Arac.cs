using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracServisTakipSistemi.Domain.Entities;

public class Arac
{
    public int Id { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public string Marka { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string AracTipi { get; set; } = string.Empty;
    public int ModelYili { get; set; }
    public double GuncelKm { get; set; }
    public DateTime SatinAlmaTarihi { get; set; }
    public int KapasiteSayisi { get; set; }
    public bool AktifMi { get; set; } = true;
    public bool BakimdaMi { get; set; } = false;
    public string? ErpAracKartNo { get; set; }
    public bool ErpyeAktarildiMi { get; set; } = false;
    public string? SasiNo { get; set; }
    public string? MotorNo { get; set; }
    public DateTime? MuayeneTarihi { get; set; }
    public DateTime? SigortaBitisTarihi { get; set; }
    public int? SubeId { get; set; }
    public int? SoforPersonelId { get; set; }
    public Personel? SoforPersonel { get; set; }

    public ICollection<AracArizaKaydi> ArizaKayitlari { get; set; } = new List<AracArizaKaydi>();
    public ICollection<BakimKaydi> BakimKayitlari { get; set; } = new List<BakimKaydi>();
    public ICollection<RiskSkoru> RiskSkorlari { get; set; } = new List<RiskSkoru>();
    public ICollection<AracAtama> AracAtamalari { get; set; } = new List<AracAtama>();
    public ICollection<Rota> Rotalar { get; set; } = new List<Rota>();
}