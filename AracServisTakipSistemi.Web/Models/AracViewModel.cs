using System.ComponentModel.DataAnnotations;

namespace AracServisTakipSistemi.Web.Models;

public class AracViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Plaka zorunludur.")]
    [Display(Name = "Plaka")]
    public string Plaka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Marka zorunludur.")]
    [Display(Name = "Marka")]
    public string Marka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model zorunludur.")]
    [Display(Name = "Model")]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "Araç tipi zorunludur.")]
    [Display(Name = "Araç Tipi")]
    public string AracTipi { get; set; } = string.Empty;

    [Range(1990, 2100, ErrorMessage = "Geçerli bir yıl girin.")]
    [Display(Name = "Model Yılı")]
    public int ModelYili { get; set; } = DateTime.Now.Year;

    [Display(Name = "Güncel Km")]
    public double GuncelKm { get; set; }

    [Display(Name = "Satın Alma Tarihi")]
    [DataType(DataType.Date)]
    public DateTime SatinAlmaTarihi { get; set; } = DateTime.Today;

    [Range(1, 200, ErrorMessage = "1 ile 200 arasında bir değer girin.")]
    [Display(Name = "Kapasite (Koltuk Sayısı)")]
    public int KapasiteSayisi { get; set; } = 14;

    [Display(Name = "Bakımda mı")]
    public bool BakimdaMi { get; set; }

    [Display(Name = "Şasi No")]
    public string? SasiNo { get; set; }

    [Display(Name = "Motor No")]
    public string? MotorNo { get; set; }

    [Display(Name = "Muayene Tarihi")]
    [DataType(DataType.Date)]
    public DateTime? MuayeneTarihi { get; set; }

    [Display(Name = "Sigorta Bitiş Tarihi")]
    [DataType(DataType.Date)]
    public DateTime? SigortaBitisTarihi { get; set; }

    [Display(Name = "Şoför")]
    public int? SoforPersonelId { get; set; }

    [Display(Name = "Aktif")]
    public bool AktifMi { get; set; } = true;
}