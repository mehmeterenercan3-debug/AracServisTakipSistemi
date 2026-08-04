using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracServisTakipSistemi.Entities.Entities;

public class AracArizaKaydi
{
    public int Id { get; set; }
    public int AracId { get; set; }
    public Arac? Arac { get; set; }
    public DateTime ArizaTarihi { get; set; }
    public string ArizaTuru { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public decimal OnarimMaliyeti { get; set; }
    public double ArizaAnindakiKm { get; set; }
}