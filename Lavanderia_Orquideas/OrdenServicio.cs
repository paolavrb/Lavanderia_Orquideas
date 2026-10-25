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
        public double PesoKg { get; set; }             // Punto 1: Peso acumulado en kilogramos
        public string CondicionPago { get; set; }      // Punto 5: "Cancelado" o "A cuenta"
        public string DetallePrendas { get; set; }
        public decimal Total { get; set; }
        public decimal ACuenta { get; set; }
        public bool IncluyeRecojo { get; set; }        // Punto 4: Opción de recojo a domicilio
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
            Console.WriteLine("\n--- REGISTRO DE NUEVA ORDEN DE SERVICIO ---");
            Orden o = new Orden();

            o.NumeroOrden = listaOrdenes.Count + 1;

            // 1. VALIDACIÓN DE NOMBRE (Solo letras y espacios)
            string clienteValido = "";
            while (true)
            {
                Console.Write("CLIENTE : ");
                string entrada = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(entrada) && System.Text.RegularExpressions.Regex.IsMatch(entrada, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                {
                    clienteValido = entrada;
                    break;
                }
                Console.WriteLine("[Error] El nombre solo debe contener letras y espacios.");
            }
            o.Cliente = clienteValido;

            // 2. VALIDACIÓN DE TELÉFONO (9 dígitos y empieza con 9)
            string telefonoValido = "";
            while (true)
            {
                Console.Write("TELÉF   : ");
                string entrada = Console.ReadLine()?.Trim();
                if (entrada.Length == 9 && long.TryParse(entrada, out _) && entrada.StartsWith("9"))
                {
                    telefonoValido = entrada;
                    break;
                }
                Console.WriteLine("[Error] El teléfono debe tener 9 dígitos y empezar con 9.");
            }
            o.Telefono = telefonoValido;

            Console.Write("DIRECC  : ");
            o.Direccion = Console.ReadLine();

            // --- OPCIÓN DE RECOJO ---
            Console.Write("¿Incluye servicio de recojo a domicilio? (s/n): ");
            string recojoInput = Console.ReadLine()?.Trim().ToLower();
            o.IncluyeRecojo = (recojoInput == "s");

            // --- CONDICIÓN DE PAGO (Simplificado a 2 opciones) ---
            Console.WriteLine("\n--- Condición de Salida del Producto ---");
            Console.WriteLine("1. Cancelado (Pago total)");
            Console.WriteLine("2. A cuenta (Con adelanto / saldo pendiente)");
            Console.Write("Seleccione condición (1-2): ");
            string opCondicion = Console.ReadLine();

            if (opCondicion == "1")
            {
                o.CondicionPago = "Cancelado";
            }
            else
            {
                o.CondicionPago = "A cuenta";
            }

            o.FechaRecepcion = DateTime.Now;
            o.FechaEntrega = DateTime.Now.AddDays(3);

            // 3. CAPTURA DE PRENDAS Y CÁLCULO DEL TOTAL
            string detallesConcatenados = "";
            decimal totalAcumulado = 0m;
            double pesoTotalAcumulado = 0;
            string continuar = "s";

            while (continuar.ToLower() == "s")
            {
                Console.WriteLine("\n--- Seleccione el Tipo de Servicio ---");
                Console.WriteLine("1. Lavado Normal (al agua)");
                Console.WriteLine("2. Lavado al Seco");
                Console.WriteLine("3. Edredón / Cobertores");
                Console.WriteLine("4. Alfombra");
                Console.WriteLine("5. Zapatillas");
                Console.WriteLine("6. Lavado por Kilo");
                Console.WriteLine("7. Otro servicio (Teñido, etc.)");
                Console.Write("Opción (1-7): ");

                string opcionServicio = Console.ReadLine();
                string tipoServicio = "";

                switch (opcionServicio)
                {
                    case "1": tipoServicio = "Lavado Normal (al agua)"; break;
                    case "2": tipoServicio = "Lavado al Seco"; break;
                    case "3": tipoServicio = "Edredón / Cobertores"; break;
                    case "4": tipoServicio = "Alfombra"; break;
                    case "5": tipoServicio = "Zapatillas"; break;
                    case "6": tipoServicio = "Lavado por Kilo"; break;
                    case "7": tipoServicio = "Otro servicio"; break;
                    default: tipoServicio = "Especial"; break;
                }

                Console.Write("Ingrese descripción de la prenda / artículo: ");
                string descripcion = Console.ReadLine();

                // --- VALIDACIÓN DE PESO (A nivel de prenda/servicio y sin negativos) ---
                double pesoPrenda = 0;
                while (true)
                {
                    Console.Write("Ingrese el peso en kilogramos (Kg) [Mínimo 0]: ");
                    if (double.TryParse(Console.ReadLine(), out pesoPrenda) && pesoPrenda >= 0)
                    {
                        break;
                    }
                    Console.WriteLine("[Error] El peso no puede ser un número negativo. Ingrese un valor válido (>= 0).");
                }

                pesoTotalAcumulado += pesoPrenda;

                // --- CÁLCULO AUTOMÁTICO SI ES LAVADO POR KILO (S/ 4.00 el kilo) O MANUAL ---
                decimal precio = 0m;
                if (opcionServicio == "6")
                {
                    precio = (decimal)pesoPrenda * 4.00m;
                    Console.WriteLine($"[Cálculo automático] {pesoPrenda} Kg x S/ 4.00 = S/ {precio:F2}");
                }
                else
                {
                    while (true)
                    {
                        Console.Write("Ingrese importe (S/): ");
                        if (decimal.TryParse(Console.ReadLine(), out precio) && precio > 0)
                        {
                            break;
                        }
                        Console.WriteLine("[Error] Ingrese un monto positivo válido mayor a 0.");
                    }
                }

                // Sumar al acumulador total
                totalAcumulado += precio;

                // Concatenar texto de detalle
                if (detallesConcatenados != "") detallesConcatenados += " | ";
                detallesConcatenados += "[" + tipoServicio + "] " + descripcion + " (" + pesoPrenda + " Kg) - S/ " + precio.ToString("F2");

                Console.Write("¿Desea agregar otra prenda/servicio? (s/n): ");
                continuar = Console.ReadLine();
            }

            // Asignación final al objeto orden
            o.DetallePrendas = detallesConcatenados;
            o.Total = totalAcumulado;
            o.PesoKg = pesoTotalAcumulado;

            // 4. VALIDACIÓN DEL MONTO "A CUENTA"
            decimal acuentaIngresado = 0m;
            while (true)
            {
                Console.Write("\nA CUENTA / ADELANTO (S/ - Máximo " + o.Total.ToString("F2") + "): ");
                if (decimal.TryParse(Console.ReadLine(), out acuentaIngresado) && acuentaIngresado >= 0 && acuentaIngresado <= o.Total)
                {
                    break;
                }
                Console.WriteLine("[Error] El adelanto debe ser entre S/ 0.00 y el total de la orden (S/ " + o.Total.ToString("F2") + ").");
            }
            o.ACuenta = acuentaIngresado;

            o.Estado = "pendiente";

            // Guardar en la lista e imprimir el ticket
            listaOrdenes.Add(o);
            ImprimirTicket(o);
        }

        public void ImprimirTicket(Orden o)
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("                ORQUÍDEAS LAVANDERÍA              ");
            Console.WriteLine(" CC. SAN FELIPE TDA 2 - RES. SAN FELIPE - JESÚS MARÍA ");
            Console.WriteLine("               Teléf.: 4610235-976344217          ");
            Console.WriteLine("==================================================");
            Console.WriteLine($"O/S N.° : {o.NumeroOrden}");
            Console.WriteLine($"CLIENTE : {o.Cliente}");
            Console.WriteLine($"TELÉF   : {o.Telefono}");
            Console.WriteLine($"DIRECC  : {o.Direccion}");
            Console.WriteLine($"RECOJO  : {(o.IncluyeRecojo ? "Sí (A domicilio)" : "No (En tienda)")}");
            Console.WriteLine($"FECHA   : {o.FechaRecepcion:dd/MM/yyyy}");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"DETALLE : {o.DetallePrendas}");
            Console.WriteLine($"PESO TOT: {o.PesoKg} Kg");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"TOTAL A PAGAR : S/ {o.Total:F2}");
            Console.WriteLine($"A CUENTA      : S/ {o.ACuenta:F2}");
            Console.WriteLine($"SALDO         : S/ {(o.Total - o.ACuenta):F2}");
            Console.WriteLine($"CONDICIÓN     : {o.CondicionPago}");
            Console.WriteLine($"FECHA ENTREGA : {o.FechaEntrega:dd/MM/yyyy}");
            Console.WriteLine("==================================================");
        }

        public void BuscarPorCliente(string nombre)
        {
            Console.WriteLine($"\n--- BÚSQUEDA POR CLIENTE: '{nombre}' ---");
            bool hallado = false;
            foreach (var o in listaOrdenes)
            {
                if (o.Cliente.ToLower().Contains(nombre.ToLower()))
                {
                    // Muestra los servicios y los detalles de pago solicitados en el punto 3
                    Console.WriteLine($"O/S: {o.NumeroOrden} | Cliente: {o.Cliente} | Servicios: {o.DetallePrendas} | Peso: {o.PesoKg}Kg | Total: S/ {o.Total:F2} | A Cuenta: S/ {o.ACuenta:F2} | Condición: {o.CondicionPago} | Estado: {o.Estado}");
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
                Console.WriteLine($"Cliente         : {o.Cliente}");
                Console.WriteLine($"Servicios/Det.  : {o.DetallePrendas}");
                Console.WriteLine($"Peso Total      : {o.PesoKg} Kg");
                Console.WriteLine($"Recojo a Dom.   : {(o.IncluyeRecojo ? "Sí" : "No")}");
                Console.WriteLine($"Total a Pagar   : S/ {o.Total:F2}");
                Console.WriteLine($"A Cuenta        : S/ {o.ACuenta:F2}");
                Console.WriteLine($"Saldo Pendiente : S/ {(o.Total - o.ACuenta):F2}");
                Console.WriteLine($"Condición Pago  : {o.CondicionPago}");
                Console.WriteLine($"Estado O/S      : {o.Estado}");
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
                Console.WriteLine($"[O/S: {o.NumeroOrden}] - Cliente: {o.Cliente} - Total: S/ {o.Total:F2} - Condición: {o.CondicionPago} - Estado: {o.Estado}");
            }
        }
    }
}