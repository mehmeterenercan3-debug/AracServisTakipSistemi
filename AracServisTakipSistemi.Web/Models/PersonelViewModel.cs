using System.ComponentModel.DataAnnotations;
using AracServisTakipSistemi.Entities.Enums;

namespace AracServisTakipSistemi.Web.Models;

public class PersonelViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad zorunludur.")]
    [Display(Name = "Ad")]
    public string Ad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [Display(Name = "Soyad")]
    public string Soyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sicil no zorunludur.")]
    [Display(Name = "Sicil No")]
    public string SicilNo { get; set; } = string.Empty;

    [Display(Name = "Personel Türü")]
    public PersonelTuru PersonelTuru { get; set; } = PersonelTuru.Calisan;

    [Display(Name = "Cinsiyet")]
    public string Cinsiyet { get; set; } = string.Empty;

    [Display(Name = "Doğum Tarihi")]
    [DataType(DataType.Date)]
    public DateTime? DogumTarihi { get; set; }

    [Display(Name = "Telefon")]
    public string? Telefon { get; set; }

    [Display(Name = "E-posta")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
    public string? Eposta { get; set; }

    [Required(ErrorMessage = "İşe giriş tarihi zorunludur.")]
    [Display(Name = "İşe Giriş Tarihi")]
    [DataType(DataType.Date)]
    public DateTime IseGirisTarihi { get; set; } = DateTime.Today;

    [Display(Name = "Vardiya")]
    public int? VardiyaId { get; set; }

    // --- Adres bilgileri (sadece ekleme sırasında kullanılıyor) ---
    [Display(Name = "Mahalle")]
    public string? Mahalle { get; set; }

    [Display(Name = "Semt")]
    public string? Semt { get; set; }

    [Display(Name = "İlçe")]
    public string? IlceAdi { get; set; }

    [Display(Name = "Şehir")]
    public string? Sehir { get; set; }

    [Display(Name = "Sokak")]
    public string? Sokak { get; set; }

    [Display(Name = "Apartman No")]
    public string? ApartmanNo { get; set; }

    [Display(Name = "Dış Kapı No")]
    public string? DisKapiNo { get; set; }
}