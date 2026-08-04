using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracServisTakipSistemi.BLL.Interfaces;

public class GeocodingSonucu
{
    public bool BasariliMi { get; set; }
    public double? Enlem { get; set; }
    public double? Boylam { get; set; }
    public string? HataMesaji { get; set; }
}

public interface IGeocodingServisi
{
    Task<GeocodingSonucu> AdresteneKoordinatBulAsync(string adres);
}