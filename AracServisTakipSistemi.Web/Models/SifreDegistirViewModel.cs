using System.ComponentModel.DataAnnotations;

namespace AracServisTakipSistemi.Web.Models;

public class SifreDegistirViewModel
{
    [Required(ErrorMessage = "Mevcut şifrenizi girin.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mevcut Şifre")]
    public string MevcutSifre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni şifrenizi girin.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalı.")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre")]
    public string YeniSifre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni şifrenizi tekrar girin.")]
    [DataType(DataType.Password)]
    [Compare(nameof(YeniSifre), ErrorMessage = "Şifreler eşleşmiyor.")]
    [Display(Name = "Yeni Şifre (Tekrar)")]
    public string YeniSifreTekrar { get; set; } = string.Empty;
}