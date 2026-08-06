namespace AracServisTakipSistemi.Web.Models;

public class ServisimViewModel
{
    public bool KayitBulunduMu { get; set; }
    public string? BilgiMesaji { get; set; }

    public string Rol { get; set; } = string.Empty; // "Sofor" ya da "Personel"
    public string AracPlaka { get; set; } = string.Empty;
    public string SoforAdSoyad { get; set; } = string.Empty;

    public ServisimYonBilgisi? Gidis { get; set; }
    public ServisimYonBilgisi? Donus { get; set; }
}

public class ServisimYonBilgisi
{
    public string RotaDurumu { get; set; } = string.Empty;
    public double ToplamMesafeKm { get; set; }
    public int TahminiSureDk { get; set; }

    // Şoför için: tüm durak listesi
    public List<ServisimDurakViewModel> TumDuraklar { get; set; } = new();

    // Personel için: sadece kendi durağı
    public int? KendiSiraNo { get; set; }
    public string? KendiVarisSaati { get; set; }
}

public class ServisimDurakViewModel
{
    public int SiraNo { get; set; }
    public string PersonelAdSoyad { get; set; } = string.Empty;
    public string VarisSaati { get; set; } = string.Empty;
}