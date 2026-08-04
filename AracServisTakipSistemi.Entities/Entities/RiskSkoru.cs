using AracServisTakipSistemi.Entities.Enums;

namespace AracServisTakipSistemi.Entities.Entities;

public class RiskSkoru
{
    public int Id { get; set; }
    public int AracId { get; set; }
    public Arac? Arac { get; set; }

    public decimal SkorDegeri { get; set; }
    public BakimRiskSeviyesi RiskSeviyesi { get; set; }
    public DateTime HesaplamaTarihi { get; set; }
    public string? OnerilenAksiyon { get; set; }
}