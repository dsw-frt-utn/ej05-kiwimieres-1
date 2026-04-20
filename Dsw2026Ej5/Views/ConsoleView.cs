using Dsw2026Ej5.Data;
using Dsw2026Ej5.Domain;

namespace Dsw2026Ej5.Views;

public class ConsoleView
{
    private static List<VehiculoViewModel> _vehiculos = Controlador.GetVehiculos();
    public static void DibujarMenu()
    {
        string? opcion = null;
        do
        {
            LimpiarPantalla();
            DibujarLinea();
            CentrarTexto("Menú Principal - Empresa de Transporte", out int _);
            DibujarLinea();
            Console.WriteLine("Elija una opción: \n");
            Console.WriteLine("1. Listar vehículos");
            Console.WriteLine("2. Agregar vehículo");
            Console.WriteLine("3. Salir");
            Console.WriteLine("\n");
            Console.WriteLine("Ingrese su opción: ");
            opcion = Console.ReadLine();
            if (opcion == "1")
            {
                Console.WriteLine("Listando vehículos...");
                ListarVehiculos();
            }
            else if (opcion == "2")
            {
                Console.WriteLine("Agregando vehículo...");
                AgregarVehiculos();
            }
        }
        while (opcion != "3");
    }

    private static void AgregarVehiculos()
    {
        LimpiarPantalla();
        DibujarLinea();
        CentrarTexto("Agregar Nuevo Vehículo", out int _);
        DibujarLinea();

        Console.Write("\nIngrese Patente: ");
        string patente = Console.ReadLine() ?? "";

        Console.Write("Ingrese Marca: ");
        string marca = Console.ReadLine() ?? "";

        Console.Write("Ingrese Modelo: ");
        string modelo = Console.ReadLine() ?? "";

        Console.Write("Ingrese Año de Fabricación: ");
        int anio;
        // TryParse intenta convertir el texto a número. Si falla, vuelve a preguntar.
        while (!int.TryParse(Console.ReadLine(), out anio))
            Console.Write("Error. Ingrese un año válido (solo números): ");

        Console.Write("Ingrese Capacidad de Carga (Kg): ");
        double capacidad;
        while (!double.TryParse(Console.ReadLine(), out capacidad))
            Console.Write("Error. Ingrese una capacidad válida: ");

        // 1. Mostrar y elegir Sucursales
        Console.WriteLine("\n--- Sucursales Disponibles ---");
        var sucursales = Persistencia.GetSucursales();
        for (int i = 0; i < sucursales.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {sucursales[i].GetCodigo()} - {sucursales[i].GetCiudad()}");
        }

        Console.Write("Seleccione el número de la sucursal: ");
        int opSucursal;
        while (!int.TryParse(Console.ReadLine(), out opSucursal) || opSucursal < 1 || opSucursal > sucursales.Count)
            Console.Write("Error. Seleccione una opción válida: ");

        Sucursal sucursalElegida = sucursales[opSucursal - 1];

        // 2. Elegir Tipo de Vehículo
        Console.WriteLine("\n--- Tipo de Vehículo ---");
        Console.WriteLine("1. Combustible");
        Console.WriteLine("2. Eléctrico");
        Console.Write("Seleccione el tipo (1 o 2): ");
        string tipoStr = Console.ReadLine() ?? "";

        Vehiculo nuevoVehiculo;

        // 3. Pedir datos específicos e instanciar
        if (tipoStr == "1") // Combustible
        {
            Console.Write("\nIngrese Kilómetros por Litro: ");
            double kmL;
            while (!double.TryParse(Console.ReadLine(), out kmL)) Console.Write("Inválido. Ingrese números: ");

            Console.Write("Ingrese Litros Extra: ");
            double lExtra;
            while (!double.TryParse(Console.ReadLine(), out lExtra)) Console.Write("Inválido. Ingrese números: ");

            nuevoVehiculo = new VehiculoCombustible(patente, marca, modelo, anio, capacidad, sucursalElegida, kmL, lExtra);
        }
        else // Eléctrico (por defecto si no elige 1)
        {
            Console.Write("\nIngrese Kwh Base: ");
            double kwh;
            while (!double.TryParse(Console.ReadLine(), out kwh)) Console.Write("Inválido. Ingrese números: ");

            nuevoVehiculo = new VehiculoElectrico(patente, marca, modelo, anio, capacidad, sucursalElegida, kwh);
        }

        // 4. Guardar en memoria
        Persistencia.AgregarVehiculo(nuevoVehiculo);

        Console.WriteLine("\n¡Vehículo agregado con éxito!");
        Console.WriteLine("Presione una tecla para volver al menú...");
        Console.ReadLine();
    }

    public static void CentrarTexto(string? texto, out int usado, int? ancho = null, bool salto = true)
    {
        texto ??= string.Empty;
        ancho ??= Console.WindowWidth;
        int largo = texto.Length;
        if (largo > ancho)
        {
            largo = ancho.Value;
            texto = texto.Substring(0, ancho.Value);
        }
        int espacios = (ancho.Value - largo) / 2;
        espacios = espacios % 2 == 0 ? espacios : espacios + 1;
        string fin = salto ? "\n" : string.Empty;
        string final = new string(' ', espacios) + texto + fin;
        Console.Write(final);
        usado = final.Length;
    }
    public static void LimpiarPantalla()
    {
        Console.Clear();
    }

    public static void DibujarLinea()
    {
        var with = Console.WindowWidth;
        for (int i = 0; i < with; i++)
        {
            Console.Write("-");
        }
    }

    private static void ListarVehiculos()
    {
        LimpiarPantalla();
        //Para asegurarnos de que se muestren los vehículos actualizados después de agregar uno nuevo, volvemos a cargar la lista desde el controlador.
        _vehiculos = Controlador.GetVehiculos();

        string[] columnas = { "Patente", "Vehículo", "Tipo", "Cap. Carga", "Km/l", "Año", "L.Extra", "Kms a recorrer" };
        DibujarEncabezado(columnas);
        DibjuarDatos(columnas.Length);
        DibujarLinea();
        Console.Write("\n");
        Console.Write("\n");
        Console.WriteLine("Presione una tecla para calcular el total de consumos...");
        Console.ReadLine();
        Dictionary<string, double> vehiculos = new Dictionary<string, double>();
        foreach (VehiculoViewModel vehiculo in _vehiculos)
        {
            vehiculos.Add(vehiculo.GetPatente(), vehiculo.GetKmARecorrer());
        }
        (double, double) totalConsumos = Controlador.CalcularConsumos(vehiculos);
        DibujarLinea();
        Console.WriteLine($"Total consumo Vehículos Eléctricos: {totalConsumos.Item1} kWh");
        Console.WriteLine($"Total consumo Vehículos Combustible: {totalConsumos.Item2:F2} Litros");
        DibujarLinea();
        Console.Write("\n");
        Console.Write("\n");
        Console.WriteLine("Presione una tecla para salir...");
        Console.ReadLine();
    }
    private static void DibujarEncabezado(params string[] columnas)
    {
        DibujarLinea();
        int ancho = Console.WindowWidth / columnas.Length;

        foreach (var columna in columnas)
        {
            Console.Write("|");
            CentrarTexto(columna, out int l, ancho - 1, false);
            Console.Write("".PadRight(ancho - 1 - l));
        }
        Console.Write("\n");
        DibujarLinea();
    }
    private static void DibjuarDatos(int columnas)
    {
        int ancho = Console.WindowWidth / columnas;
        foreach (var vehiculo in _vehiculos)
        {
            Console.Write("|");
            CentrarTexto(vehiculo.GetPatente(), out int l, ancho - 1, false);
            Console.Write("".PadRight(ancho - 1 - l));
            Console.Write("|");
            CentrarTexto(vehiculo.GetVehiculo(), out l, ancho - 1, false);
            Console.Write("".PadRight(ancho - 1 - l));
            Console.Write("|");
            CentrarTexto(vehiculo.GetTipo(), out l, ancho - 1, false);
            Console.Write("".PadRight(ancho - 1 - l));
            Console.Write("|");
            CentrarTexto(vehiculo.GetCapacidadCarga().ToString(), out l, ancho - 1, false);
            Console.Write("".PadRight(ancho - 1 - l));
            Console.Write("|");
            CentrarTexto(vehiculo.GetKmPorLitro().ToString(), out l, ancho - 1, false);
            Console.Write("".PadRight(ancho - 1 - l));
            Console.Write("|");
            CentrarTexto(vehiculo.GetAnio().ToString(), out l, ancho - 1, false);
            Console.Write("".PadRight(ancho - 1 - l));
            Console.Write("|");
            CentrarTexto(vehiculo.GetLitrosExtra().ToString(), out l, ancho - 1, false);
            Console.Write("".PadRight(ancho - 1 - l));
            Console.Write("|");
            CentrarTexto(vehiculo.GetKmARecorrer().ToString(), out l, ancho - 1, false);
            Console.Write("".PadRight(ancho - 1 - l));
        }
    }
}
