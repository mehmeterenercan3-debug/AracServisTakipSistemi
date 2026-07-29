using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracServisTakipSistemi.Application.Interfaces;

public interface IMesafeHesaplayici
{
    double MesafeHesaplaKm(double enlem1, double boylam1, double enlem2, double boylam2);
}