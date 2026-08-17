namespace AracServisTakipSistemi.Web.Models;

public class MaliyetRaporuViewModel
{
    public decimal BuAyToplamMaliyet { get; set; }
    public decimal BuAyArizaMaliyeti { get; set; }
    public decimal BuAyBakimMaliyeti { get; set; }

    public List<AracMaliyetOzeti> EnMaliyetliUcArac { get; set; } = new();
    public List<AracMaliyetOzeti> TumAraclarMaliyetDetayi { get; set; } = new();

    public decimal FiloToplamMaliyeti { get; set; }
    public decimal FiloOrtalamaKmBasinaMaliyet { get; set; }
}

public class AracMaliyetOzeti
{
    public int AracId { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public string MarkaModel { get; set; } = string.Empty;
    public decimal ToplamMaliyet { get; set; }
    public double GuncelKm { get; set; }
    public decimal KmBasinaMaliyet { get; set; }
    public int ArizaSayisi { get; set; }
    public int BakimSayisi { get; set; }
}