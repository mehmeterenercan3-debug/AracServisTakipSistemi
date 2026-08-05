namespace AracServisTakipSistemi.Web.Models;

public class RotaViewModel
{
    public int Id { get; set; }
    public int AracId { get; set; }
    public string AracPlaka { get; set; } = string.Empty;
    public string Durum { get; set; } = string.Empty;
    public double ToplamMesafeKm { get; set; }
    public int TahminiSureDk { get; set; }
    public DateTime RotaTarihi { get; set; }
    public List<RotaDurakViewModel> Duraklar { get; set; } = new();
}

public class RotaDurakViewModel
{
    public int SiraNo { get; set; }
    public string PersonelAdSoyad { get; set; } = string.Empty;
    public string VarisSaati { get; set; } = string.Empty;
}