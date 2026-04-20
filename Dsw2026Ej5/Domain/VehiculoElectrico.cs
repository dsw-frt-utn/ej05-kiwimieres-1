using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Ej5.Domain;

public class VehiculoElectrico : Vehiculo
{
    private double kwhBase;

    public VehiculoElectrico(string patente, string marca, string modelo, int anio, double capacidadCarga, 
        Sucursal sucursal, double kwhBase) : base(VehiculoTipo.Electrico, patente, marca, modelo, anio, capacidadCarga, sucursal)
    {
        this.kwhBase = kwhBase;
    }

    public double GetKwhBase()
    {
        return kwhBase;
    }

    public override double CalcularConsumo(double kilometros)
    {
        //Cálculo base cada 100 km
        double total = (kilometros / 100) * kwhBase;

        //Recargo usando el getter público de la clase padre
        if (GetCapacidadCarga() >= 1200)
        {
            total = total * 1.15;
        }

        return total;
    }
}
