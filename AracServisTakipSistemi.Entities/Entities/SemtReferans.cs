using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracServisTakipSistemi.Entities.Entities;

public class SemtReferans
{
    public int Id { get; set; }
    public string SemtAdi { get; set; } = string.Empty;
    public double MerkezEnlem { get; set; }
    public double MerkezBoylam { get; set; }
}