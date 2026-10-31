using System;
using System.Collections.Generic;

namespace LavanderiaOrquideas
{
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
        public string Estado { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public DateTime FechaEntrega { get; set; }

        public double PesoKg { get; set; }
        public bool IncluyeRecojo { get; set; }
        public string CondicionPago { get; set; }
        public string MetodoPago { get; set; }

        public string ParaArchivo()
        {
            return NumeroOrden + ";" + Cliente + ";" + Telefono + ";" + Direccion + ";" + DetallePrendas + ";" + Total + ";" + ACuenta + ";" + Estado + ";" + FechaRecepcion.ToString("yyyy-MM-dd") + ";" + FechaEntrega.ToString("yyyy-MM-dd") + ";" + PesoKg + ";" + IncluyeRecojo + ";" + CondicionPago + ";" + MetodoPago;
        }
    }

    public class OrdenService
    {
        private List<Orden> listaOrdenes = new List<Orden>();

        public List<Orden> ObtenerTodas()
        {
            return listaOrdenes;
        }

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
            Console.WriteLine();
            Console.WriteLine("--- REGISTRO DE NUEVA ORDEN DE SERVICIO ---");
            Orden o = new Orden();

            o.NumeroOrden = listaOrdenes.Count + 1;

            string clienteValido = "";
            while (true)
            {
                Console.Write("CLIENTE : ");
                string entrada = Console.ReadLine();
                if (entrada != null) entrada = entrada.Trim();

                if (!string.IsNullOrEmpty(entrada) && System.Text.RegularExpressions.Regex.IsMatch(entrada, "^[a-zA-ZáéíóúÁÉÍÓÚñÑ\\s]+$"))
                {
                    clienteValido = entrada;
                    break;
                }
                Console.WriteLine("[Error] El nombre solo debe contener letras y espacios.");
            }
            o.Cliente = clienteValido;

            string telefonoValido = "";
            while (true)
            {
                Console.Write("TELÉF   : ");
                string entrada = Console.ReadLine();
                if (entrada != null) entrada = entrada.Trim();

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

            Console.Write("¿Incluye servicio de recojo a domicilio? (s/n): ");
            string recojoInput = Console.ReadLine();
            if (recojoInput != null) recojoInput = recojoInput.Trim().ToLower();
            o.IncluyeRecojo = (recojoInput == "s");

            o.FechaRecepcion = DateTime.Now;
            o.FechaEntrega = DateTime.Now.AddDays(3);

            string detallesConcatenados = "";
            decimal totalAcumulado = 0m;
            double pesoTotalAcumulado = 0;
            string continuar = "s";

            while (continuar.ToLower() == "s")
            {
                Console.WriteLine();
                Console.WriteLine("--- Seleccione el Tipo de Servicio ---");
                Console.WriteLine("1. Lavado Normal (al agua)");
                Console.WriteLine("2. Lavado al Seco");
                Console.WriteLine("3. Edredón / Cobertores");
                Console.WriteLine("4. Alfombra");
                Console.WriteLine("5. Zapatillas");
                Console.WriteLine("6. Lavado por Kilo (S/ 4.00 el kg)");
                Console.WriteLine("7. Otro servicio (Teñido, etc.)");
                Console.Write("Opción (1-7): ");

                string opcionServicio = Console.ReadLine();
                string tipoServicio = "";
                decimal precio = 0m;
                double pesoPrenda = 0;

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

                if (opcionServicio == "6")
                {
                    while (true)
                    {
                        Console.Write("Ingrese cuántos kilogramos (Kg) son: ");
                        if (double.TryParse(Console.ReadLine(), out pesoPrenda) && pesoPrenda > 0)
                        {
                            break;
                        }
                        Console.WriteLine("[Error] Ingrese un peso válido mayor a 0.");
                    }

                    precio = (decimal)pesoPrenda * 4.00m;
                    Console.WriteLine("-> Cálculo automático por Kilo (" + pesoPrenda + " Kg x S/ 4.00) = S/ " + precio.ToString("F2"));
                }
                else
                {
                    while (true)
                    {
                        Console.Write("Ingrese el peso en kilogramos (Kg) para este ítem [Mínimo 0]: ");
                        if (double.TryParse(Console.ReadLine(), out pesoPrenda) && pesoPrenda >= 0)
                        {
                            break;
                        }
                        Console.WriteLine("[Error] El peso no puede ser negativo.");
                    }

                    while (true)
                    {
                        Console.Write("Ingrese importe (S/): ");
                        if (decimal.TryParse(Console.ReadLine(), out precio) && precio > 0)
                        {
                            break;
                        }
                        Console.WriteLine("[Error] Ingrese un monto válido mayor a 0.");
                    }
                }

                pesoTotalAcumulado += pesoPrenda;
                totalAcumulado += precio;

                if (detallesConcatenados != "") detallesConcatenados += " | ";
                detallesConcatenados += "[" + tipoServicio + "] " + descripcion + " (" + pesoPrenda + " Kg) - S/ " + precio.ToString("F2");

                while (true)
                {
                    Console.Write("¿Desea agregar otra prenda/servicio? (s/n): ");
                    string respuesta = Console.ReadLine();
                    if (respuesta != null) respuesta = respuesta.Trim().ToLower();

                    if (respuesta == "s" || respuesta == "n")
                    {
                        continuar = respuesta;
                        break;
                    }
                    Console.WriteLine("[Error] Opción no válida. Ingrese 's' para sí o 'n' para no.");
                }
            }

            o.DetallePrendas = detallesConcatenados;
            o.Total = totalAcumulado;
            o.PesoKg = pesoTotalAcumulado;

            Console.WriteLine();
            Console.WriteLine("--- Condición de Salida del Pago ---");
            Console.WriteLine("1. Cancelado (Completo)");
            Console.WriteLine("2. Con Adelanto (A cuenta)");
            Console.Write("Seleccione condición (1-2): ");
            string opCondicion = Console.ReadLine();
            if (opCondicion == "1")
            {
                o.CondicionPago = "Cancelado";
            }
            else
            {
                o.CondicionPago = "Con Adelanto";
            }

            Console.WriteLine();
            Console.WriteLine("--- Seleccione el Método de Pago ---");
            Console.WriteLine("1. Yape / Plin");
            Console.WriteLine("2. Transferencia");
            Console.WriteLine("3. Efectivo");
            Console.Write("Seleccione opción (1-3): ");
            string opMetodo = Console.ReadLine();
            switch (opMetodo)
            {
                case "1": o.MetodoPago = "Yape/Plin"; break;
                case "2": o.MetodoPago = "Transferencia"; break;
                case "3": o.MetodoPago = "Efectivo"; break;
                default: o.MetodoPago = "Efectivo"; break;
            }

            decimal acuentaIngresado = 0m;
            if (o.CondicionPago == "Con Adelanto")
            {
                while (true)
                {
                    Console.WriteLine();
                    Console.Write("MONTO A CUENTA (S/ - Máximo " + o.Total.ToString("F2") + "): ");
                    if (decimal.TryParse(Console.ReadLine(), out acuentaIngresado) && acuentaIngresado > 0 && acuentaIngresado < o.Total)
                    {
                        break;
                    }
                    Console.WriteLine("[Error] El adelanto debe ser mayor a 0 y menor al total.");
                }
            }
            else
            {
                acuentaIngresado = o.Total;
            }
            o.ACuenta = acuentaIngresado;

            o.Estado = "pendiente";

            listaOrdenes.Add(o);
            ImprimirTicket(o);
        }

        public void ImprimirTicket(Orden o)
        {
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine("                ORQUÍDEAS LAVANDERÍA              ");
            Console.WriteLine(" CC. SAN FELIPE TDA 2 - RES. SAN FELIPE - JESÚS MARÍA ");
            Console.WriteLine("               Teléf.: 4610235-976344217          ");
            Console.WriteLine("==================================================");
            Console.WriteLine("O/S N.°       : " + o.NumeroOrden);
            Console.WriteLine("CLIENTE       : " + o.Cliente);
            Console.WriteLine("TELÉF         : " + o.Telefono);
            Console.WriteLine("DIRECC        : " + o.Direccion);
            Console.WriteLine("RECOJO DOM.   : " + (o.IncluyeRecojo ? "Sí" : "No"));
            Console.WriteLine("FECHA         : " + o.FechaRecepcion.ToString("dd/MM/yyyy"));
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("DETALLE       : " + o.DetallePrendas);
            Console.WriteLine("PESO TOTAL    : " + o.PesoKg + " Kg");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("TOTAL A PAGAR : S/ " + o.Total.ToString("F2"));
            Console.WriteLine("A CUENTA      : S/ " + o.ACuenta.ToString("F2"));
            Console.WriteLine("SALDO         : S/ " + (o.Total - o.ACuenta).ToString("F2"));
            Console.WriteLine("CONDICIÓN     : " + o.CondicionPago);
            Console.WriteLine("MÉTODO PAGO   : " + o.MetodoPago);
            Console.WriteLine("FECHA ENTREGA : " + o.FechaEntrega.ToString("dd/MM/yyyy"));
            Console.WriteLine("==================================================");
        }

        public void BuscarPorCliente(string nombre)
        {
            Console.WriteLine();
            Console.WriteLine("--- BÚSQUEDA POR CLIENTE: '" + nombre + "' ---");
            bool hallado = false;
            foreach (var o in listaOrdenes)
            {
                if (o.Cliente.ToLower().Contains(nombre.ToLower()))
                {
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine("[O/S N.° " + o.NumeroOrden + "]");
                    Console.WriteLine("Cliente        : " + o.Cliente);
                    Console.WriteLine("Detalle        : " + o.DetallePrendas);
                    Console.WriteLine("Peso Total     : " + o.PesoKg + " Kg");
                    Console.WriteLine("Total a Pagar  : S/ " + o.Total.ToString("F2"));
                    Console.WriteLine("A Cuenta       : S/ " + o.ACuenta.ToString("F2"));
                    Console.WriteLine("Saldo Pendiente: S/ " + (o.Total - o.ACuenta).ToString("F2"));
                    Console.WriteLine("Condición Pago : " + o.CondicionPago);
                    Console.WriteLine("Método de Pago : " + o.MetodoPago);
                    Console.WriteLine("Estado O/S     : " + o.Estado.ToUpper());
                    Console.WriteLine("--------------------------------------------------");
                    hallado = true;
                }
            }
            if (!hallado) Console.WriteLine("No se encontraron órdenes para este cliente.");
        }

        public void BuscarPorCodigo(int codigo)
        {
            Console.WriteLine();
            Console.WriteLine("--- BÚSQUEDA POR CÓDIGO O/S: " + codigo + " ---");
            Orden o = listaOrdenes.Find(x => x.NumeroOrden == codigo);
            if (o != null)
            {
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("[O/S N.° " + o.NumeroOrden + "]");
                Console.WriteLine("Cliente        : " + o.Cliente);
                Console.WriteLine("Detalle        : " + o.DetallePrendas);
                Console.WriteLine("Peso Total     : " + o.PesoKg + " Kg");
                Console.WriteLine("Total a Pagar  : S/ " + o.Total.ToString("F2"));
                Console.WriteLine("Adelanto       : S/ " + o.ACuenta.ToString("F2"));
                Console.WriteLine("Saldo Pendiente: S/ " + (o.Total - o.ACuenta).ToString("F2"));
                Console.WriteLine("Condición Pago : " + o.CondicionPago);
                Console.WriteLine("Método de Pago : " + o.MetodoPago);
                Console.WriteLine("Recojo Dom.    : " + (o.IncluyeRecojo ? "Sí" : "No"));
                Console.WriteLine("Estado O/S     : " + o.Estado.ToUpper());
                Console.WriteLine("--------------------------------------------------");
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
                Console.WriteLine();
                Console.WriteLine("--> ¡Éxito! La orden O/S " + codigo + " cambió su estado a: '" + nuevoEstado + "'.");
                return true;
            }
            Console.WriteLine();
            Console.WriteLine("--> Orden no encontrada.");
            return false;
        }

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
            Console.WriteLine();
            Console.WriteLine("--> Lista de órdenes ordenada correctamente por número de O/S (Burbuja).");
        }

        public void ListarTodas()
        {
            Console.WriteLine();
            Console.WriteLine("--- LISTADO GENERAL DE ÓRDENES ---");
            if (listaOrdenes.Count == 0)
            {
                Console.WriteLine("No hay órdenes registradas.");
                return;
            }
            foreach (var o in listaOrdenes)
            {
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("[O/S N.° " + o.NumeroOrden + "] - Cliente: " + o.Cliente);
                Console.WriteLine("Detalle : " + o.DetallePrendas);
                Console.WriteLine("Total   : S/ " + o.Total.ToString("F2") + " | Adelanto: S/ " + o.ACuenta.ToString("F2") + " (" + o.CondicionPago + ") - Pago: " + o.MetodoPago);
                Console.WriteLine("Estado  : " + o.Estado.ToUpper());
                Console.WriteLine("--------------------------------------------------");
            }
        }
    }
}