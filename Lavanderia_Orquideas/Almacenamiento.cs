using System;
using System.Collections.Generic;
using System.IO;

namespace LavanderiaOrquideas
{
    public static class Almacenamiento
    {
        private static string rutaArchivo = "ordenes_lavanderia.txt";

        // Guardar la lista de órdenes en archivo de texto
        public static void GuardarArchivo(List<Orden> listaOrdenes)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(rutaArchivo))
                {
                    foreach (var o in listaOrdenes)
                    {
                        // Guardamos cada propiedad separada por punto y coma (;) incluyendo fecha/hora exacta y método de pago
                        sw.WriteLine($"{o.NumeroOrden};{o.Cliente};{o.Telefono};{o.Direccion};{o.DetallePrendas};{o.Total};{o.ACuenta};{o.Estado};{o.FechaRecepcion:yyyy-MM-dd HH:mm:ss};{o.FechaEntrega:yyyy-MM-dd HH:mm:ss};{o.PesoKg};{o.IncluyeRecojo};{o.CondicionPago};{o.MetodoPago}");
                    }
                }
                Console.WriteLine("\n--> [Éxito] Órdenes guardadas correctamente en el archivo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Error al guardar el archivo]: {ex.Message}");
            }
        }

        // Cargar las órdenes desde el archivo de texto
        public static List<Orden> CargarArchivo()
        {
            List<Orden> lista = new List<Orden>();

            try
            {
                if (!File.Exists(rutaArchivo))
                {
                    return lista;
                }

                string[] lineas = File.ReadAllLines(rutaArchivo);
                foreach (string linea in lineas)
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    string[] partes = linea.Split(';');
                    if (partes.Length >= 14) // Verificamos que tenga todos los campos (incluyendo fecha/hora y método de pago)
                    {
                        Orden o = new Orden
                        {
                            NumeroOrden = int.Parse(partes[0]),
                            Cliente = partes[1],
                            Telefono = partes[2],
                            Direccion = partes[3],
                            DetallePrendas = partes[4],
                            Total = decimal.Parse(partes[5]),
                            ACuenta = decimal.Parse(partes[6]),
                            Estado = partes[7],
                            FechaRecepcion = DateTime.Parse(partes[8]),
                            FechaEntrega = DateTime.Parse(partes[9]),
                            PesoKg = double.Parse(partes[10]),
                            IncluyeRecojo = bool.Parse(partes[11]),
                            CondicionPago = partes[12],
                            MetodoPago = partes[13]
                        };
                        lista.Add(o);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Error al cargar el archivo]: {ex.Message}");
            }

            return lista;
        }
    }
}