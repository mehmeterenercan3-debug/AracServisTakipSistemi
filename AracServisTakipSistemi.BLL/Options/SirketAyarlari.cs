namespace AracServisTakipSistemi.BLL.Options;

public class SirketAyarlari
{
    public string Ad { get; set; } = "Merkez Ofis";
    public double Enlem { get; set; }
    public double Boylam { get; set; }

    // Personel, vardiya başlangıcından bu kadar dakika önce şirkette olmalı
    public int GidisVarisTamponDk { get; set; } = 15;

    // Şoför, vardiya bitişinden bu kadar dakika sonra şirketten ayrılır
    public int DonusKalkisTamponDk { get; set; } = 15;

    // Geocode edilen bir adres, en yakın bölge merkezine bu mesafeden (km) daha uzaksa
    // atama yapılmaz — muhtemelen adres yanlış/eksik girilmiş ya da geocoding yanlış eşleşmiştir.
    public double MaksimumBolgeMesafesiKm { get; set; } = 100;
}