using AracServisTakipSistemi.Entities.Enums;

namespace AracServisTakipSistemi.Entities.Entities;

public class Personel
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
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
    public ServisDurumu ServisDurumu { get; set; } = ServisDurumu.Atanmis;
    public DateTime IseGirisTarihi { get; set; }
    public DateTime? IstenCikisTarihi { get; set; }
    public string? ErpPersonelKodu { get; set; }
    public bool ErpyeAktarildiMi { get; set; } = false;
    public int? SubeId { get; set; }
    public int? VardiyaId { get; set; }
    public Vardiya? Vardiya { get; set; }
    public int? BolgeId { get; set; }
    public Bolge? Bolge { get; set; }

    public ICollection<PersonelAdres> Adresler { get; set; } = new List<PersonelAdres>();
    public ICollection<RotaDuragi> RotaDuraklari { get; set; } = new List<RotaDuragi>();
}