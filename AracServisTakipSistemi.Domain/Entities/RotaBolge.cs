using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracServisTakipSistemi.Domain.Entities;

public class RotaBolge
{
    public int Id { get; set; }
    public int RotaId { get; set; }
    public Rota? Rota { get; set; }
    public int BolgeId { get; set; }
    public Bolge? Bolge { get; set; }
}
