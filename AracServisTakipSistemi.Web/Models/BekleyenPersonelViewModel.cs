using AracServisTakipSistemi.Entities.Entities;

namespace AracServisTakipSistemi.Web.Models;

public class BekleyenPersonelViewModel
{
    public List<Personel> KapasiteNedeniyleBekleyenler { get; set; } = new();
    public List<Personel> KoordinatiEksikOlanlar { get; set; } = new();
}