using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracServisTakipSistemi.Domain.Entities;

public class AracAtama
{
    public int Id { get; set; }
    public int AracId { get; set; }
    public Arac? Arac { get; set; }
    public int PersonelId { get; set; }
    public Personel? Personel { get; set; }
    public DateTime AtamaTarihi { get; set; }
    public DateTime? AtamaBitisTarihi { get; set; }
    public bool AktifMi { get; set; } = true;
}