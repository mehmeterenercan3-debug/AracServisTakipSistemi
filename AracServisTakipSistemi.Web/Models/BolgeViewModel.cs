using System.ComponentModel.DataAnnotations;

namespace AracServisTakipSistemi.Web.Models;

public class BolgeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Bölge kodu zorunludur.")]
    [Display(Name = "Bölge Kodu")]
    public string BolgeKodu { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bölge adı zorunludur.")]
    [Display(Name = "Bölge Adı")]
    public string BolgeAdi { get; set; } = string.Empty;

    [Display(Name = "Merkez Enlem")]
    public double? MerkezEnlem { get; set; }

    [Display(Name = "Merkez Boylam")]
    public double? MerkezBoylam { get; set; }

    [Range(1, 200, ErrorMessage = "1 ile 200 arasında bir değer girin.")]
    [Display(Name = "Minimum Personel Eşiği")]
    public int MinPersonelEsigi { get; set; } = 15;

    [Range(0, 50, ErrorMessage = "0 ile 50 arasında bir değer girin.")]
    [Display(Name = "Kapasite Tamponu")]
    public int KapasiteTamponu { get; set; } = 5;

    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    [Display(Name = "Aktif")]
    public bool AktifMi { get; set; } = true;
}