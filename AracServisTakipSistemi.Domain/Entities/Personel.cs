using AracServisTakipSistemi.Domain.Enums;

namespace AracServisTakipSistemi.Domain.Entities;

public class Personel
{
    public int Id { get; set; }
    public string AdSoyad { get; set; } = string.Empty;
    public string SicilNo { get; set; } = string.Empty;
    public PersonelTuru PersonelTuru { get; set; } = PersonelTuru.Calisan;
    public string Cinsiyet { get; set; } = string.Empty;
    public DateTime? DogumTarihi { get; set; }
    public string Adres { get; set; } = string.Empty;
    public double? Enlem { get; set; }
    public double? Boylam { get; set; }
    public string Semt { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;
    public DateTime IseGirisTarihi { get; set; }
    public DateTime? IstenCikisTarihi { get; set; }
    public string? ErpPersonelKodu { get; set; }
    public bool ErpyeAktarildiMi { get; set; } = false;
    public int? SubeId { get; set; }
    public int? VardiyaId { get; set; }
    public Vardiya? Vardiya { get; set; }

    public ICollection<AracAtama> AracAtamalari { get; set; } = new List<AracAtama>();
    public ICollection<RotaDuragi> RotaDuraklari { get; set; } = new List<RotaDuragi>();
}