using AracServisTakipSistemi.Entities.Entities;
using AracServisTakipSistemi.Entities.Enums;

namespace AracServisTakipSistemi.BLL.DTOs;

public class RotaSonucu
{
    public int AracId { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public int VardiyaId { get; set; }
    public string VardiyaAdi { get; set; } = string.Empty;
    public RotaYonu Yon { get; set; } = RotaYonu.Gidis;
    public List<int> BolgeIdleri { get; set; } = new();
    public List<Personel> ZiyaretSirasi { get; set; } = new();

    // ZiyaretSirasi ile aynı sırada, her durağın tahmini varış saati
    public List<TimeSpan> VarisSaatleri { get; set; } = new();

    public int ToplamPersonelSayisi => ZiyaretSirasi.Count;
    public double ToplamMesafeKm { get; set; }
    public int TahminiToplamSureDk { get; set; }
    public TimeSpan KalkisSaati { get; set; }
}

public class RotaHesaplamaSonucu
{
    public List<RotaSonucu> Rotalar { get; set; } = new();
    public List<Personel> BeklemedeKalanPersoneller { get; set; } = new();
    public List<Personel> KoordinatiEksikPersoneller { get; set; } = new();
    public List<string> Uyarilar { get; set; } = new();
    public bool KapasiteYetersiz { get; set; } = false;
    public int OnerilenEkAracSayisi { get; set; } = 0;
}