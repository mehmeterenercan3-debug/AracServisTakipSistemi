using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracServisTakipSistemi.Entities.Entities;

public class Vardiya
{
    public int Id { get; set; }
    public string VardiyaAdi { get; set; } = string.Empty;
    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }
    public bool AktifMi { get; set; } = true;

    public ICollection<Personel> Personeller { get; set; } = new List<Personel>();
}