using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.Entities.Enums;

namespace AracServisTakipSistemi.BLL.DTOs;

public class BakimRiskSonucu
{
    public int AracId { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public BakimRiskSeviyesi RiskSeviyesi { get; set; }
    public int RiskPuani { get; set; }
    public List<string> Oneriler { get; set; } = new();
}