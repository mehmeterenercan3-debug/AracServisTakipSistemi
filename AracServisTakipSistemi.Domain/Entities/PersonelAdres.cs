namespace AracServisTakipSistemi.Domain.Entities;

public class PersonelAdres
{
    public int Id { get; set; }
    public int PersonelId { get; set; }
    public Personel? Personel { get; set; }

    public string AdresTuru { get; set; } = "İkametgah";
    public DateTime BaslangicTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public bool AktifMi { get; set; } = true;

    public string Mahalle { get; set; } = string.Empty;
    public string Semt { get; set; } = string.Empty;
    public string IlceAdi { get; set; } = string.Empty;
    public string Sehir { get; set; } = string.Empty;
    public string Sokak { get; set; } = string.Empty;
    public string ApartmanNo { get; set; } = string.Empty;
    public string DisKapiNo { get; set; } = string.Empty;

    public double? Enlem { get; set; }
    public double? Boylam { get; set; }
    public DateTime? GeocodeTarihi { get; set; }
}