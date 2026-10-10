using System;
using System.Collections.Generic;

namespace LavanderiaOrquideas
{
    // Clase que representa el modelo de una Orden de Servicio
    public class Orden
    {
        public int NumeroOrden { get; set; }
        public string Cliente { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string DetallePrendas { get; set; }
        public decimal Total { get; set; }
        public decimal ACuenta { get; set; }
        public decimal Saldo => Total - ACuenta;
        public string Estado { get; set; } // "pendiente" o "entregado"
        public DateTime FechaRecepcion { get; set; }
        public DateTime FechaEntrega { get; set; }

        public string ParaArchivo()
        {
            return $"{NumeroOrden};{Cliente};{Telefono};{Direccion};{DetallePrendas};{Total};{ACuenta};{Estado};{FechaRecepcion:yyyy-MM-dd};{FechaEntrega:yyyy-MM-dd}";
        }
    }

    public class OrdenService
    {
        private List<Orden> listaOrdenes = new List<Orden>();

        public List<Orden> ObtenerTodas() => listaOrdenes;

        public void CargarOrdenes(List<Orden> ordenes)
        {
            listaOrdenes = ordenes ?? new List<Orden>();
        }

        public int GenerarNuevoCodigo()
        {
            return (listaOrdenes.Count + 1) * 10;
        }

        public void RegistrarOrdenDirecta(Orden orden)
        {
            listaOrdenes.Add(orden);
        }

        public void RegistrarNuevaOrdenInteractiva()
        {
            Console.WriteLine("\n--- NUEVA ORDEN DE SERVICIO ---");
            Orden ord = new Orden();
            ord.NumeroOrden = GenerarNuevoCodigo();
            ord.FechaRecepcion = DateTime.Now;

            ord.Cliente = Utilidades.LeerNombreValido("Nombre del cliente: ");
            ord.Telefono = Utilidades.LeerTelefonoValido("Teléfono (9 dígitos): ");
            Console.Write("Dirección: ");
            ord.Direccion = Console.ReadLine();

            int cantPrendas = Utilidades.LeerEnteroPositivo("¿Cuántas prendas o bultos desea registrar?: ");
            decimal totalTemp = 0;
            List<string> detalles = new List<string>();

            for (int i = 1; i <= cantPrendas; i++)
                Console.WriteLine("Tipo de Servicio: 1. Al seco | 2. Normal | 3. Edredones | 4. Zapatillas");
            Console.Write("Opción: ");
            string opcServ = Console.ReadLine();

            string servicio;
            switch (opcServ)
            {
                case "1":
                    servicio = "Al seco";
                    break;
                case "2":
                    servicio = "Normal";
                    break;
                case "3":
                    servicio = "Edredones";
                    break;
                case "4":
                    servicio = "Zapatillas";
                    break;
                default:
                    servicio = "General";
                    break;
            }

            ord.DetallePrendas = string.Join(" | ", detalles);
            ord.Total = totalTemp;
            ord.Estado = "pendiente";

            Console.WriteLine($"\nTOTAL A PAGAR: S/ {ord.Total:F2}");
            Console.Write("Ingrese monto A CUENTA S/ (0 si no deja adelanto): ");
            if (decimal.TryParse(Console.ReadLine(), out decimal acuenta))
            {
                ord.ACuenta = acuenta;
            }

            ord.FechaEntrega = DateTime.Now.AddDays(3);

            listaOrdenes.Add(ord);
            ImprimirTicket(ord);
        }

        public void ImprimirTicket(Orden ord)
        {
            Console.Clear();
            Console.WriteLine("==================================================");
            Console.WriteLine("               ORQUÍDEAS LAVANDERÍA               ");
            Console.WriteLine(" CC. SAN FELIPE TDA 2 - RES. SAN FELIPE - JESÚS MARÍA");
            Console.WriteLine("             Teléf.: 4610235-976344217            ");
            Console.WriteLine("==================================================");
            Console.WriteLine($"O/S N.° : {ord.NumeroOrden}");
            Console.WriteLine($"CLIENTE : {ord.Cliente}");
            Console.WriteLine($"TELÉF   : {ord.Telefono}");
            Console.WriteLine($"DIRECC  : {ord.Direccion}");
            Console.WriteLine($"FECHA   : {Utilidades.FormatearFecha(ord.FechaRecepcion)}");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"DETALLE : {ord.DetallePrendas}");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"TOTAL A PAGAR : S/ {ord.Total:F2}");
            Console.WriteLine($"A CUENTA      : S/ {ord.ACuenta:F2}");
            Console.WriteLine($"SALDO         : S/ {ord.Saldo:F2}");
            Console.WriteLine($"FECHA ENTREGA : {Utilidades.FormatearFecha(ord.FechaEntrega)}");
            Console.WriteLine("==================================================\n");
        }

        public void BuscarPorCliente(string nombre)
        {
            Console.WriteLine($"\n--- BÚSQUEDA POR CLIENTE: '{nombre}' ---");
            bool hallado = false;
            foreach (var o in listaOrdenes)
            {
                if (o.Cliente.ToLower().Contains(nombre.ToLower()))
                {
                    Console.WriteLine($"O/S: {o.NumeroOrden} | Cliente: {o.Cliente} | Total: S/ {o.Total:F2} | Estado: {o.Estado}");
                    hallado = true;
                }
            }
            if (!hallado) Console.WriteLine("No se encontraron órdenes para este cliente.");
        }

        public void BuscarPorCodigo(int codigo)
        {
            Console.WriteLine($"\n--- BÚSQUEDA POR CÓDIGO O/S: {codigo} ---");
            Orden o = listaOrdenes.Find(x => x.NumeroOrden == codigo);
            if (o != null)
            {
                Console.WriteLine($"Cliente: {o.Cliente}\nDetalle: {o.DetallePrendas}\nTotal: S/ {o.Total:F2}\nEstado: {o.Estado}");
            }
            else
            {
                Console.WriteLine("Orden no encontrada.");
            }
        }

        public bool ActualizarEstado(int codigo, string nuevoEstado)
        {
            Orden o = listaOrdenes.Find(x => x.NumeroOrden == codigo);
            if (o != null)
            {
                o.Estado = nuevoEstado;
                Console.WriteLine($"\n--> ¡Éxito! La orden O/S {codigo} cambió su estado a: '{nuevoEstado}'.");
                return true;
            }
            Console.WriteLine("\n--> Orden no encontrada.");
            return false;
        }

        // Ordenamiento por Algoritmo de la Burbuja por Número de Orden
        public void OrdenarPorBurbuja()
        {
            int n = listaOrdenes.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (listaOrdenes[j].NumeroOrden > listaOrdenes[j + 1].NumeroOrden)
                    {
                        var temp = listaOrdenes[j];
                        listaOrdenes[j] = listaOrdenes[j + 1];
                        listaOrdenes[j + 1] = temp;
                    }
                }
            }
            Console.WriteLine("\n--> Lista de órdenes ordenada correctamente por número de O/S (Burbuja).");
        }

        public void ListarTodas()
        {
            Console.WriteLine("\n--- LISTADO GENERAL DE ÓRDENES ---");
            if (listaOrdenes.Count == 0)
            {
                Console.WriteLine("No hay órdenes registradas.");
                return;
            }
            foreach (var o in listaOrdenes)
            {
                Console.WriteLine($"[O/S: {o.NumeroOrden}] - Cliente: {o.Cliente} - Total: S/ {o.Total:F2} - Estado: {o.Estado}");
            }
        }
    }
}
