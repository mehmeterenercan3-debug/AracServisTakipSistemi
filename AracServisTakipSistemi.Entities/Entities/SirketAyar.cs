namespace AracServisTakipSistemi.Entities.Entities;

// artık admin panelinden değiştirilebilmesi için veritabanına taşınmış hali.
public class SirketAyar
{
    public int Id { get; set; }
    public string Ad { get; set; } = "Merkez Ofis";
    public double Enlem { get; set; }
    public double Boylam { get; set; }

    // Personel, vardiya başlangıcından bu kadar dakika önce şirkette olmalı
    public int GidisVarisTamponDk { get; set; } = 15;

    // Şoför, vardiya bitişinden bu kadar dakika sonra şirketten ayrılır
    public int DonusKalkisTamponDk { get; set; } = 15;

    // Geocode edilen bir adres, en yakın bölge merkezine bu mesafeden (km) daha uzaksa atama yapılmaz
    public double MaksimumBolgeMesafesiKm { get; set; } = 100;
}