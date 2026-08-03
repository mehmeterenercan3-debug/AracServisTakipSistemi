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
    public string? AnneAdi { get; set; }
    public string? BabaAdi { get; set; }
    public string? KanGrubu { get; set; }
    public string? Telefon { get; set; }
    public string? Eposta { get; set; }
    public bool AktifMi { get; set; } = true;
    public DateTime IseGirisTarihi { get; set; }
    public DateTime? IstenCikisTarihi { get; set; }
    public string? ErpPersonelKodu { get; set; }
    public bool ErpyeAktarildiMi { get; set; } = false;
    public int? SubeId { get; set; }
    public int? VardiyaId { get; set; }
    public Vardiya? Vardiya { get; set; }

    public ICollection<PersonelAdres> Adresler { get; set; } = new List<PersonelAdres>();
    public ICollection<AracAtama> AracAtamalari { get; set; } = new List<AracAtama>();
    public ICollection<RotaDuragi> RotaDuraklari { get; set; } = new List<RotaDuragi>();
}