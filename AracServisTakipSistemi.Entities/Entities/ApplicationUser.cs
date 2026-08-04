using Microsoft.AspNetCore.Identity;

namespace AracServisTakipSistemi.Entities.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public int? PersonelId { get; set; }
    public Personel? Personel { get; set; }
}