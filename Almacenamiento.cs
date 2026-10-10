using System;
using System.Collections.Generic;
using System.IO;

namespace LavanderiaOrquideas
{
    public class Almacenamiento
    {
        private const string RutaArchivo = "ordenes_lavanderia.txt";

        public static void GuardarArchivo(List<Orden> ordenes)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(RutaArchivo, false))
                {
                    foreach (var ord in ordenes)
                    {
                        sw.WriteLine(ord.ParaArchivo());
                    }
                }
                Console.WriteLine($"\n--> Se guardaron {ordenes.Count} registro(s) en el archivo local '{RutaArchivo}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar en archivo: {ex.Message}");
            }
        }

        public static List<Orden> CargarArchivo()
        {
            List<Orden> ordenes = new List<Orden>();
            if (!File.Exists(RutaArchivo)) return ordenes;

            try
            {
                string[] lineas = File.ReadAllLines(RutaArchivo);
                foreach (var linea in lineas)
                {
                    string[] datos = linea.Split(';');
                    if (datos.Length >= 10)
                    {
                        Orden ord = new Orden
                        {
                            NumeroOrden = int.Parse(datos[0]),
                            Cliente = datos[1],
                            Telefono = datos[2],
                            Direccion = datos[3],
                            DetallePrendas = datos[4],
                            Total = decimal.Parse(datos[5]),
                            ACuenta = decimal.Parse(datos[6]),
                            Estado = datos[7],
                            FechaRecepcion = DateTime.Parse(datos[8]),
                            FechaEntrega = DateTime.Parse(datos[9])
                        };
                        ordenes.Add(ord);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Advertencia al leer archivo local: {ex.Message}");
            }
            return ordenes;
        }
    }
}
