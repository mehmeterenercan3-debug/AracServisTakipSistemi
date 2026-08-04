using AracServisTakipSistemi.Entities.Enums;

namespace AracServisTakipSistemi.Entities.Entities;

public class Rota
{
    public int Id { get; set; }
    public int AracId { get; set; }
    public Arac? Arac { get; set; }
    public RotaDurumu Durum { get; set; } = RotaDurumu.Onaylandi;
    public DateTime RotaTarihi { get; set; }
    public double ToplamMesafeKm { get; set; }
    public int TahminiSureDk { get; set; }
    public bool AktifMi { get; set; } = true;
    public bool ErpyeAktarildiMi { get; set; } = false;

    public ICollection<RotaBolge> Bolgeler { get; set; } = new List<RotaBolge>();
    public ICollection<RotaDuragi> Duraklar { get; set; } = new List<RotaDuragi>();
}

public class RotaDuragi
{
    public int Id { get; set; }
    public int RotaId { get; set; }
    public Rota? Rota { get; set; }
    public int PersonelId { get; set; }
    public Personel? Personel { get; set; }
    public int SiraNo { get; set; }
    public TimeSpan TahminiVarisSaati { get; set; }
}