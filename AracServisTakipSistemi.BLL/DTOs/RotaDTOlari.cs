using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.BLL.DTOs;

public class RotaSonucu
{
    public int AracId { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public int VardiyaId { get; set; }
    public string VardiyaAdi { get; set; } = string.Empty;
    public List<int> BolgeIdleri { get; set; } = new();
    public List<Personel> ZiyaretSirasi { get; set; } = new();
    public int ToplamPersonelSayisi => ZiyaretSirasi.Count;
    public int TahminiToplamSureDk { get; set; }
    public TimeSpan GidisKalkisSaati { get; set; }
    public TimeSpan DonusKalkisSaati { get; set; }
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