using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracServisTakipSistemi.Entities.Entities;

public class Bolge
{
    public int Id { get; set; }
    public string BolgeKodu { get; set; } = string.Empty;
    public string BolgeAdi { get; set; } = string.Empty;
    public double? MerkezEnlem { get; set; }
    public double? MerkezBoylam { get; set; }
    public int MinPersonelEsigi { get; set; } = 15;
    public int KapasiteTamponu { get; set; } = 5;
    public string? Aciklama { get; set; }
    public bool AktifMi { get; set; } = true;

    public ICollection<Personel> Personeller { get; set; } = new List<Personel>();
    public ICollection<RotaBolge> RotaBolgeleri { get; set; } = new List<RotaBolge>();
}