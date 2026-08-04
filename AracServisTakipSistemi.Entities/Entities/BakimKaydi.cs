namespace AracServisTakipSistemi.Entities.Entities;

public class BakimKaydi
{
    public int Id { get; set; }
    public int AracId { get; set; }
    public Arac? Arac { get; set; }

    public DateTime BakimTarihi { get; set; }
    public string BakimTuru { get; set; } = string.Empty;
    public int? SonrakiBakimKm { get; set; }
    public DateTime? SonrakiBakimTarihi { get; set; }
    public decimal Maliyet { get; set; }
    public string? Aciklama { get; set; }
}