using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AracServisTakipSistemi.BLL.Interfaces;

namespace AracServisTakipSistemi.BLL.Services;

public class HaversineMesafeHesaplayici : IMesafeHesaplayici
{
    private const double DunyaYaricapiKm = 6371.0;

    public double MesafeHesaplaKm(double enlem1, double boylam1, double enlem2, double boylam2)
    {
        var dEnlem = ToRadyan(enlem2 - enlem1);
        var dBoylam = ToRadyan(boylam2 - boylam1);

        var a = Math.Sin(dEnlem / 2) * Math.Sin(dEnlem / 2) +
                Math.Cos(ToRadyan(enlem1)) * Math.Cos(ToRadyan(enlem2)) *
                Math.Sin(dBoylam / 2) * Math.Sin(dBoylam / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return DunyaYaricapiKm * c;
    }

    private static double ToRadyan(double aci) => aci * Math.PI / 180.0;
}
