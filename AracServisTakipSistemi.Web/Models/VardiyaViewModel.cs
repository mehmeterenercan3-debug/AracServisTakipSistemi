using System.ComponentModel.DataAnnotations;

namespace AracServisTakipSistemi.Web.Models;

public class VardiyaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vardiya adı zorunludur.")]
    [Display(Name = "Vardiya Adı")]
    public string VardiyaAdi { get; set; } = string.Empty;

    [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
    [DataType(DataType.Time)]
    [Display(Name = "Başlangıç Saati")]
    public TimeSpan BaslangicSaati { get; set; }

    [Required(ErrorMessage = "Bitiş saati zorunludur.")]
    [DataType(DataType.Time)]
    [Display(Name = "Bitiş Saati")]
    public TimeSpan BitisSaati { get; set; }

    [Display(Name = "Aktif")]
    public bool AktifMi { get; set; } = true;
}