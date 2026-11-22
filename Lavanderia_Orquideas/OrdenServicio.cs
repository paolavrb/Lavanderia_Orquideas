using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
        public string TipoComprobante { get; set; }
        public string Ruc { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }

        public string ParaArchivo()
        {
            return NumeroOrden + ";" +
                   (Cliente ?? "") + ";" +
                   (Telefono ?? "") + ";" +
                   (Direccion ?? "") + ";" +
                   (DetallePrendas ?? "") + ";" +
                   Total + ";" +
                   ACuenta + ";" +
                   (Estado ?? "") + ";" +
                   FechaRecepcion.ToString("yyyy-MM-dd HH:mm:ss") + ";" +
                   FechaEntrega.ToString("yyyy-MM-dd HH:mm:ss") + ";" +
                   PesoKg + ";" +
                   IncluyeRecojo + ";" +
                   (CondicionPago ?? "") + ";" +
                   (MetodoPago ?? "") + ";" +
                   (TipoComprobante ?? "") + ";" +
                   (Ruc ?? "") + ";" +
                   (TipoDocumento ?? "") + ";" +
                   (NumeroDocumento ?? "");
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
            int maxNumero = listaOrdenes.Count > 0 ? listaOrdenes.Max(x => x.NumeroOrden) : 0;
            return maxNumero + 1;
        }

        public void RegistrarOrdenDirecta(Orden orden)
        {
            if (orden != null)
            {
                listaOrdenes.Add(orden);
            }
        }

        public void RegistrarNuevaOrdenInteractiva()
        {
            Console.WriteLine();
            Console.WriteLine("--- REGISTRO DE NUEVA ORDEN DE SERVICIO ---");
            Orden o = new Orden();

            o.NumeroOrden = GenerarNuevoCodigo();

            Console.WriteLine();
            Console.WriteLine("--- Tipo de Documento del Cliente ---");
            string opcionDocumento;
            while (true)
            {
                Console.WriteLine("1. DNI");
                Console.WriteLine("2. RUC");
                Console.WriteLine("3. Carné de Extranjería");
                Console.Write("Seleccione documento (1-3): ");
                opcionDocumento = (Console.ReadLine() ?? "").Trim();
                if (opcionDocumento == "1" || opcionDocumento == "2" || opcionDocumento == "3") break;
                Console.WriteLine("[Error] Opción no válida. Elija una de las opciones mostradas.");
            }

            if (opcionDocumento == "1")
            {
                while (true)
                {
                    Console.Write("Ingrese DNI (8 dígitos, no puede empezar con 0): ");
                    string dni = (Console.ReadLine() ?? "").Trim();
                    if (Regex.IsMatch(dni, @"^[1-9][0-9]{7}$"))
                    {
                        o.TipoDocumento = "DNI";
                        o.NumeroDocumento = dni;
                        o.Ruc = "";
                        break;
                    }
                    Console.WriteLine("[Error] El DNI debe tener 8 dígitos y comenzar con un número del 1 al 9.");
                }
            }
            else if (opcionDocumento == "2")
            {
                while (true)
                {
                    Console.Write("Ingrese RUC (11 dígitos, empieza con 10, 15 o 20): ");
                    string ruc = (Console.ReadLine() ?? "").Trim();
                    if (EsRucValido(ruc))
                    {
                        o.TipoDocumento = "RUC";
                        o.NumeroDocumento = ruc;
                        o.Ruc = ruc;
                        break;
                    }
                    Console.WriteLine("[Error] RUC inválido. Debe tener 11 dígitos positivos y empezar con 10, 15 o 20.");
                }
            }
            else
            {
                while (true)
                {
                    Console.Write("Ingrese Carné de Extranjería (9 dígitos): ");
                    string carne = (Console.ReadLine() ?? "").Trim();
                    if (Regex.IsMatch(carne, @"^[0-9]{9}$"))
                    {
                        o.TipoDocumento = "Carné de Extranjería";
                        o.NumeroDocumento = carne;
                        o.Ruc = "";
                        break;
                    }
                    Console.WriteLine("[Error] El Carné de Extranjería debe tener exactamente 9 dígitos.");
                }
            }

            string telefonoValido = "";
            while (true)
            {
                Console.Write("TELÉFONO : ");
                string entrada = (Console.ReadLine() ?? "").Trim();
                if (entrada.Length == 9 && long.TryParse(entrada, out long numTel) && numTel > 0 && entrada.StartsWith("9"))
                {
                    telefonoValido = entrada;
                    break;
                }
                Console.WriteLine("[Error] El teléfono debe tener 9 dígitos y empezar con 9.");
            }
            o.Telefono = telefonoValido;

            Orden clienteExistente = listaOrdenes.FindLast(x => (x.Telefono ?? "") == o.Telefono);

            if (clienteExistente != null && !string.IsNullOrWhiteSpace(clienteExistente.Cliente))
            {
                o.Cliente = clienteExistente.Cliente;

                Console.WriteLine();
                Console.WriteLine("[¡Cliente Frecuente Detectado!]: Bienvenido de nuevo, " + o.Cliente + ".");
                Console.WriteLine("Dirección registrada: " + (clienteExistente.Direccion ?? ""));

                Console.Write("¿Desea mantener esta dirección? (s/n): ");
                string respuestaDir = (Console.ReadLine() ?? "").Trim().ToLower();

                if (respuestaDir == "n")
                {
                    Console.Write("Ingrese la NUEVA dirección: ");
                    o.Direccion = (Console.ReadLine() ?? "").Trim();
                }
                else
                {
                    o.Direccion = clienteExistente.Direccion ?? "";
                    Console.WriteLine("-> Se mantiene la dirección anterior.");
                }
            }
            else
            {
                string clienteValido = "";
                while (true)
                {
                    Console.Write("CLIENTE (Nuevo): ");
                    string entrada = (Console.ReadLine() ?? "").Trim();
                    if (!string.IsNullOrEmpty(entrada) && Regex.IsMatch(entrada, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                    {
                        clienteValido = entrada;
                        break;
                    }
                    Console.WriteLine("[Error] El nombre solo debe contener letras y espacios.");
                }
                o.Cliente = clienteValido;

                Console.Write("DIRECC  : ");
                o.Direccion = (Console.ReadLine() ?? "").Trim();
            }

            Console.WriteLine();
            Console.WriteLine("--- Tipo de Comprobante ---");
            if (o.TipoDocumento != "RUC")
            {
                o.TipoComprobante = "Boleta";
                Console.WriteLine("Con " + o.TipoDocumento + " se emitirá una boleta. Para factura, seleccione RUC.");
            }
            else
            {
                string opcionComprobante;
                while (true)
                {
                    Console.WriteLine("1. Boleta");
                    Console.WriteLine("2. Factura");
                    Console.Write("Seleccione comprobante (1-2): ");
                    opcionComprobante = (Console.ReadLine() ?? "").Trim();
                    if (opcionComprobante == "1" || opcionComprobante == "2") break;
                    Console.WriteLine("[Error] Opción no válida. Elija una de las opciones mostradas.");
                }
                o.TipoComprobante = opcionComprobante == "2" ? "Factura" : "Boleta";
            }

            string detallesConcatenados = "";
            decimal totalAcumulado = 0m;
            double pesoTotalAcumulado = 0;
            string continuar = "s";

            while (continuar.ToLower() == "s")
            {
                Console.WriteLine();
                Console.WriteLine("--- Seleccione el Tipo de Servicio ---");
                Console.WriteLine("1. Lavado al Seco");
                Console.WriteLine("2. Edredón / Cobertores");
                Console.WriteLine("3. Alfombra");
                Console.WriteLine("4. Zapatillas");
                Console.WriteLine("5. Lavado por Kilo (S/ 4.00 el kg)");
                Console.WriteLine("6. Teñidos (precio según prenda)");
                Console.WriteLine("7. Planchado (precio según prenda)");
                Console.Write("Opción (1-7): ");
                string opcionServicio = (Console.ReadLine() ?? "").Trim();
                string tipoServicio = "";
                decimal precio = 0m;
                double pesoPrenda = 0;
                string descripcion;

                switch (opcionServicio)
                {
                    case "1": tipoServicio = "Lavado al Seco"; break;
                    case "2": tipoServicio = "Edredón / Cobertores"; break;
                    case "3": tipoServicio = "Alfombra"; break;
                    case "4": tipoServicio = "Zapatillas"; break;
                    case "5": tipoServicio = "Lavado por Kilo"; break;
                    case "6": tipoServicio = "Teñido"; break;
                    case "7": tipoServicio = "Planchado"; break;
                    default: tipoServicio = "Especial"; break;
                }

                if (opcionServicio == "5")
                {
                    descripcion = "Ropa";
                    while (true)
                    {
                        Console.Write("¿Cuántos kilos de ropa trae? ");
                        if (double.TryParse(Console.ReadLine(), out pesoPrenda) && pesoPrenda > 0) break;
                        Console.WriteLine("[Error] Ingrese un peso válido mayor a 0.");
                    }
                    precio = (decimal)pesoPrenda * 4.00m;
                    Console.WriteLine("-> " + pesoPrenda + " kg de ropa x S/ 4.00 = S/ " + precio.ToString("F2"));
                }
                else if (opcionServicio == "2")
                {
                    Console.WriteLine("Seleccione tipo: 1. Cubrecamas  2. Edredón  3. Frazada");
                    string tipo = (Console.ReadLine() ?? "").Trim();
                    switch (tipo)
                    {
                        case "1": descripcion = "Cubrecamas"; break;
                        case "2": descripcion = "Edredón"; break;
                        case "3": descripcion = "Frazada"; break;
                        default: descripcion = "Edredón"; Console.WriteLine("Opción no válida; se seleccionó Edredón."); break;
                    }

                    if (tipo == "2")
                        Console.WriteLine("Seleccione tamaño: 1. 1 1/2  2. 2P  3. K  4. Q  5. De plumas");
                    else
                        Console.WriteLine("Seleccione tamaño: 1. 1 1/2  2. 2P  3. K  4. Q");

                    string medida = (Console.ReadLine() ?? "").Trim();
                    string nombreMedida;
                    switch (medida)
                    {
                        case "1": nombreMedida = "1 1/2"; precio = 24m; break;
                        case "2": nombreMedida = "2P"; precio = 26m; break;
                        case "3": nombreMedida = "K"; precio = 28m; break;
                        case "4": nombreMedida = "Q"; precio = 28m; break;
                        case "5":
                            if (tipo == "2") { nombreMedida = "De plumas"; precio = 30m; }
                            else { nombreMedida = "1 1/2"; precio = 24m; Console.WriteLine("Opción no válida; se aplicó 1 1/2."); }
                            break;
                        default: nombreMedida = "1 1/2"; precio = 24m; Console.WriteLine("Opción no válida; se aplicó 1 1/2."); break;
                    }
                    descripcion += " - " + nombreMedida;
                    Console.WriteLine("-> Precio calculado: S/ " + precio.ToString("F2"));
                }
                else if (opcionServicio == "3")
                {
                    double largo, ancho;
                    while (true)
                    {
                        Console.Write("Ingrese largo de la alfombra en metros: ");
                        if (double.TryParse(Console.ReadLine(), out largo) && largo > 0) break;
                        Console.WriteLine("[Error] Ingrese una medida válida mayor a 0.");
                    }
                    while (true)
                    {
                        Console.Write("Ingrese ancho de la alfombra en metros: ");
                        if (double.TryParse(Console.ReadLine(), out ancho) && ancho > 0) break;
                        Console.WriteLine("[Error] Ingrese una medida válida mayor a 0.");
                    }
                    descripcion = "Alfombra " + largo + " m x " + ancho + " m (" + (largo * ancho) + " m2)";
                    precio = (decimal)(largo * ancho) * 13m;
                    Console.WriteLine("-> " + (largo * ancho) + " m2 x S/ 13.00 = S/ " + precio.ToString("F2"));
                }
                else if (opcionServicio == "1" || opcionServicio == "6" || opcionServicio == "7")
                {
                    Console.Write("Ingrese la prenda (por ejemplo: polo, pantalón, saco o terno): ");
                    descripcion = (Console.ReadLine() ?? "").Trim();
                    bool tarifaEncontrada;

                    if (opcionServicio == "1")
                    {
                        tarifaEncontrada = TryObtenerPrecioLavadoSeco(descripcion, out precio);
                        tipoServicio = "Lavado al Seco";
                    }
                    else if (opcionServicio == "7")
                    {
                        tarifaEncontrada = TryObtenerPrecioPlanchado(descripcion, out precio);
                        tipoServicio = "Planchado";
                    }
                    else
                    {
                        tarifaEncontrada = TryObtenerPrecioTenido(descripcion, out precio);
                        tipoServicio = "Teñido";
                    }

                    if (tarifaEncontrada)
                    {
                        Console.WriteLine("-> Precio de " + tipoServicio + " para " + descripcion + ": S/ " + precio.ToString("F2"));
                    }
                    else
                    {
                        Console.WriteLine("No hay una tarifa referencial registrada para esa prenda.");
                        while (true)
                        {
                            Console.Write("Ingrese importe (S/): ");
                            if (decimal.TryParse(Console.ReadLine(), out precio) && precio > 0) break;
                            Console.WriteLine("[Error] Ingrese un monto válido mayor a 0.");
                        }
                    }
                }
                else
                {
                    Console.Write("Ingrese descripción de la prenda / artículo: ");
                    descripcion = (Console.ReadLine() ?? "").Trim();
                    while (true)
                    {
                        Console.Write("Ingrese el peso en kilogramos (Kg) para este ítem [Mínimo 0]: ");
                        if (double.TryParse(Console.ReadLine(), out pesoPrenda) && pesoPrenda >= 0) break;
                        Console.WriteLine("[Error] El peso no puede ser negativo.");
                    }
                    while (true)
                    {
                        Console.Write("Ingrese importe (S/): ");
                        if (decimal.TryParse(Console.ReadLine(), out precio) && precio > 0) break;
                        Console.WriteLine("[Error] Ingrese un monto válido mayor a 0.");
                    }
                }

                pesoTotalAcumulado += pesoPrenda;
                totalAcumulado += precio;
                if (detallesConcatenados != "") detallesConcatenados += " | ";
                detallesConcatenados += "[" + tipoServicio + "] " + descripcion + (pesoPrenda > 0 ? " (" + pesoPrenda + " Kg)" : "") + " - S/ " + precio.ToString("F2");

                while (true)
                {
                    Console.Write("¿Desea agregar otra prenda/servicio? (s/n): ");
                    string respuesta = (Console.ReadLine() ?? "").Trim().ToLower();
                    if (respuesta == "s" || respuesta == "n")
                    {
                        continuar = respuesta;
                        break;
                    }
                    Console.WriteLine("[Error] Opción no válida. Ingrese 's' para sí o 'n' para no.");
                }
            }

            Console.WriteLine();
            Console.Write("¿Desea delivery a domicilio? (s/n): ");
            string deliveryInput;
            while (true)
            {
                deliveryInput = (Console.ReadLine() ?? "").Trim().ToLower();
                if (deliveryInput == "s" || deliveryInput == "n") break;
                Console.Write("Opción no válida. Responda s o n: ");
            }

            o.IncluyeRecojo = (deliveryInput == "s");
            if (o.IncluyeRecojo)
            {
                totalAcumulado += 10m;
                if (detallesConcatenados != "") detallesConcatenados += " | ";
                detallesConcatenados += "[Delivery a domicilio] - S/ 10.00";
            }

            o.FechaRecepcion = DateTime.Now;
            while (true)
            {
                Console.Write("Ingrese fecha y hora de entrega (dd/MM/yyyy HH:mm): ");
                string fechaHoraTexto = (Console.ReadLine() ?? "").Trim();
                DateTime fechaHoraEntrega;
                bool formatoValido = DateTime.TryParseExact(
                    fechaHoraTexto,
                    "dd/MM/yyyy HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out fechaHoraEntrega);

                if (formatoValido && fechaHoraEntrega >= DateTime.Now)
                {
                    o.FechaEntrega = fechaHoraEntrega;
                    break;
                }
                Console.WriteLine("[Error] Ingrese una fecha y hora válidas, igual o posterior al momento actual.");
            }

            o.DetallePrendas = detallesConcatenados;
            o.Total = totalAcumulado;
            o.PesoKg = pesoTotalAcumulado;

            string opCondicion;
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("--- Condición de Salida del Pago ---");
                Console.WriteLine("1. Cancelado (Completo)");
                Console.WriteLine("2. Con Adelanto (A cuenta)");
                Console.Write("Seleccione condición (1-2): ");
                opCondicion = (Console.ReadLine() ?? "").Trim();
                if (opCondicion == "1" || opCondicion == "2") break;
                Console.WriteLine("[Error] Opción no válida. Elija una de las opciones mostradas.");
            }

            o.CondicionPago = opCondicion == "1" ? "Cancelado" : "Con Adelanto";

            Console.WriteLine();
            Console.WriteLine("--- Seleccione el Método de Pago ---");
            Console.WriteLine("1. Yape / Plin");
            Console.WriteLine("2. Transferencia");
            Console.WriteLine("3. Efectivo");
            Console.Write("Seleccione opción (1-3): ");
            string opMetodo = (Console.ReadLine() ?? "").Trim();
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

        private static bool TryObtenerPrecioLavadoSeco(string descripcion, out decimal precio)
        {
            string p = (descripcion ?? "").Trim().ToLowerInvariant();
            bool delicado = p.Contains("delic") || p.Contains("grues") || p.Contains("seda");
            if (p.Contains("terno")) precio = delicado ? 32m : 24m;
            else if (p.Contains("saco") && (p.Contains("largo") || p.Contains("3/4"))) precio = delicado ? 32m : 24m;
            else if (p.Contains("saco") || p.Contains("blazer")) precio = delicado ? 24m : 18m;
            else if (p.Contains("camisa") || p.Contains("blusa")) precio = p.Contains("larga") || p.Contains("m/l") || p.Contains("ml") ? (delicado ? 12m : 9m) : (delicado ? 9m : 6m);
            else if (p.Contains("casaca")) precio = delicado ? 18m : 12m;
            else if (p.Contains("buzo")) precio = delicado ? 24m : 18m;
            else if (p.Contains("chompa")) precio = p.Contains("especial") ? 18m : (delicado ? 12m : 9m);
            else if (p.Contains("pantalon") || p.Contains("pantalón") || p.Contains("falda")) precio = delicado ? 12m : 9m;
            else if (p.Contains("polo") || p.Contains("polera")) precio = p.Contains("larga") || p.Contains("m/l") || p.Contains("ml") ? (delicado ? 12m : 9m) : (delicado ? 9m : 6m);
            else if (p.Contains("conjunto")) precio = delicado ? 32m : 24m;
            else if (p.Contains("vestido") && p.Contains("largo")) precio = delicado ? 32m : 24m;
            else if (p.Contains("vestido")) precio = delicado ? 24m : 18m;
            else { precio = 0m; return false; }
            return true;
        }

        private static bool TryObtenerPrecioPlanchado(string descripcion, out decimal precio)
        {
            string p = (descripcion ?? "").Trim().ToLowerInvariant();
            if (p.Contains("terno")) precio = 12m;
            else if (p.Contains("saco") && (p.Contains("largo") || p.Contains("3/4"))) precio = 12m;
            else if (p.Contains("saco") || p.Contains("blazer")) precio = 9m;
            else if (p.Contains("camisa")) precio = p.Contains("larga") || p.Contains("m/l") || p.Contains("ml") ? 4m : 3m;
            else if (p.Contains("polo")) precio = p.Contains("larga") || p.Contains("m/l") || p.Contains("ml") ? 4m : 3m;
            else if (p.Contains("pantalon") || p.Contains("pantalón") || p.Contains("chompa") || p.Contains("falda") || p.Contains("blusa")) precio = 4m;
            else if (p.Contains("casaca")) precio = 6m;
            else if (p.Contains("corbata")) precio = 2m;
            else if (p.Contains("vestido") && p.Contains("largo")) precio = 16m;
            else if (p.Contains("vestido")) precio = 12m;
            else { precio = 0m; return false; }
            return true;
        }

        private static bool TryObtenerPrecioTenido(string descripcion, out decimal precio)
        {
            string p = (descripcion ?? "").Trim().ToLowerInvariant();
            if (p.Contains("blazer") || (p.Contains("saco") && p.Contains("corduroy"))) precio = 25m;
            else if (p.Contains("vestido")) precio = 25m;
            else if (p.Contains("casaca") && p.Contains("jean")) precio = 25m;
            else if (p.Contains("camisa")) precio = 15m;
            else if (p.Contains("blusa") || p.Contains("bufanda") || p.Contains("chaleco") || p.Contains("chompa") || p.Contains("falda") || p.Contains("short")) precio = 15m;
            else if (p.Contains("casaca") || p.Contains("pantalon") || p.Contains("pantalón") || p.Contains("polera") || p.Contains("saco")) precio = 20m;
            else if (p.Contains("polo")) precio = 15m;
            else { precio = 0m; return false; }
            return true;
        }

        private static bool EsRucValido(string ruc)
        {
            if (string.IsNullOrWhiteSpace(ruc))
                return false;

            ruc = ruc.Trim();

            if (!Regex.IsMatch(ruc, @"^[0-9]{11}$"))
                return false;

            string prefijo = ruc.Substring(0, 2);
            return prefijo == "10" || prefijo == "15" || prefijo == "20";
        }

        public void ImprimirTicket(Orden o)
        {
            if (o == null) return;

            try
            {
                if (Console.BufferWidth < 350) Console.BufferWidth = 350;
            }
            catch { }

            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine("                ORQUÍDEAS LAVANDERÍA              ");
            Console.WriteLine(" CC. SAN FELIPE TDA 2 - RES. SAN FELIPE - JESÚS MARÍA ");
            Console.WriteLine("               Teléf.: 4610235-976344217          ");
            Console.WriteLine("==================================================");
            Console.WriteLine("COMPROBANTE   : " + (string.IsNullOrWhiteSpace(o.TipoComprobante) ? "BOLETA" : o.TipoComprobante.ToUpperInvariant()));
            Console.WriteLine("O/S N.°       : " + o.NumeroOrden);
            Console.WriteLine("CLIENTE       : " + (o.Cliente ?? ""));
            string documentoTicket = !string.IsNullOrWhiteSpace(o.NumeroDocumento) ? o.NumeroDocumento : (o.Ruc ?? "");
            string tipoDocumentoTicket = !string.IsNullOrWhiteSpace(o.TipoDocumento) ? o.TipoDocumento : (!string.IsNullOrWhiteSpace(o.Ruc) ? "RUC" : "");
            if (!string.IsNullOrWhiteSpace(documentoTicket)) Console.WriteLine(tipoDocumentoTicket.PadRight(14) + ": " + documentoTicket);
            Console.WriteLine("TELÉF         : " + (o.Telefono ?? ""));
            Console.WriteLine("DIRECC        : " + (o.Direccion ?? ""));
            Console.WriteLine("DELIVERY DOM. : " + (o.IncluyeRecojo ? "Sí - S/ 10.00" : "No"));
            Console.WriteLine("FECHA/HORA    : " + o.FechaRecepcion.ToString("dd/MM/yyyy HH:mm"));
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("DETALLE       : " + (o.DetallePrendas ?? ""));
            Console.WriteLine("PESO TOTAL    : " + o.PesoKg + " Kg");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("TOTAL A PAGAR : S/ " + o.Total.ToString("F2"));
            Console.WriteLine("A CUENTA      : S/ " + o.ACuenta.ToString("F2"));
            Console.WriteLine("SALDO         : S/ " + (o.Total - o.ACuenta).ToString("F2"));
            Console.WriteLine("CONDICIÓN     : " + (o.CondicionPago ?? ""));
            Console.WriteLine("MÉTODO PAGO   : " + (o.MetodoPago ?? ""));
            Console.WriteLine("FECHA ENTREGA : " + o.FechaEntrega.ToString("dd/MM/yyyy HH:mm"));
            Console.WriteLine("==================================================");
        }

        public void BuscarPorCliente(string nombre)
        {
            string filtro = (nombre ?? "").Trim().ToLower();
            Console.WriteLine();
            Console.WriteLine("--- BÚSQUEDA POR CLIENTE: '" + filtro + "' ---");
            bool hallado = false;
            foreach (var o in listaOrdenes)
            {
                string clienteActual = (o.Cliente ?? "").ToLower();
                if (clienteActual.Contains(filtro))
                {
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine("[O/S N.° " + o.NumeroOrden + "]");
                    Console.WriteLine("Cliente        : " + (o.Cliente ?? ""));
                    Console.WriteLine("F. Recepción   : " + o.FechaRecepcion.ToString("dd/MM/yyyy HH:mm"));
                    Console.WriteLine("Detalle        : " + (o.DetallePrendas ?? ""));
                    Console.WriteLine("Peso Total     : " + o.PesoKg + " Kg");
                    Console.WriteLine("Total a Pagar  : S/ " + o.Total.ToString("F2"));
                    Console.WriteLine("A Cuenta       : S/ " + o.ACuenta.ToString("F2"));
                    Console.WriteLine("Saldo Pendiente: S/ " + (o.Total - o.ACuenta).ToString("F2"));
                    Console.WriteLine("Condición Pago : " + (o.CondicionPago ?? ""));
                    Console.WriteLine("Método de Pago : " + (o.MetodoPago ?? ""));
                    Console.WriteLine("Estado O/S     : " + (o.Estado ?? "").ToUpper());
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
                Console.WriteLine("Cliente        : " + (o.Cliente ?? ""));
                Console.WriteLine("F. Recepción   : " + o.FechaRecepcion.ToString("dd/MM/yyyy HH:mm"));
                Console.WriteLine("Detalle        : " + (o.DetallePrendas ?? ""));
                Console.WriteLine("Peso Total     : " + o.PesoKg + " Kg");
                Console.WriteLine("Total a Pagar  : S/ " + o.Total.ToString("F2"));
                Console.WriteLine("Adelanto       : S/ " + o.ACuenta.ToString("F2"));
                Console.WriteLine("Saldo Pendiente: S/ " + (o.Total - o.ACuenta).ToString("F2"));
                Console.WriteLine("Condición Pago : " + (o.CondicionPago ?? ""));
                Console.WriteLine("Método de Pago : " + (o.MetodoPago ?? ""));
                Console.WriteLine("Recojo Dom.    : " + (o.IncluyeRecojo ? "Sí" : "No"));
                Console.WriteLine("F. Est. Entrega: " + o.FechaEntrega.ToString("dd/MM/yyyy HH:mm"));
                Console.WriteLine("Estado O/S     : " + (o.Estado ?? "").ToUpper());
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
            if (o == null || string.IsNullOrWhiteSpace(nuevoEstado))
            {
                return false;
            }

            o.Estado = nuevoEstado.Trim().ToLower();
            return true;
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
                        Orden temp = listaOrdenes[j];
                        listaOrdenes[j] = listaOrdenes[j + 1];
                        listaOrdenes[j + 1] = temp;
                    }
                }
            }
        }

        public void ListarTodas()
        {
            Console.WriteLine();
            Console.WriteLine("--- LISTADO DE ÓRDENES DE SERVICIO ---");
            if (listaOrdenes.Count == 0)
            {
                Console.WriteLine("No hay órdenes registradas.");
                return;
            }

            foreach (var o in listaOrdenes)
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("[O/S N.° " + o.NumeroOrden + "]");
                Console.WriteLine("Cliente        : " + (o.Cliente ?? ""));
                Console.WriteLine("Teléfono       : " + (o.Telefono ?? ""));
                Console.WriteLine("F. Recepción   : " + o.FechaRecepcion.ToString("dd/MM/yyyy HH:mm"));
                Console.WriteLine("F. Entrega     : " + o.FechaEntrega.ToString("dd/MM/yyyy HH:mm"));
                Console.WriteLine("Detalle        : " + (o.DetallePrendas ?? ""));
                Console.WriteLine("Peso Total     : " + o.PesoKg + " Kg");
                Console.WriteLine("Total a Pagar  : S/ " + o.Total.ToString("F2"));
                Console.WriteLine("A Cuenta       : S/ " + o.ACuenta.ToString("F2"));
                Console.WriteLine("Saldo Pendiente: S/ " + (o.Total - o.ACuenta).ToString("F2"));
                Console.WriteLine("Condición Pago : " + (o.CondicionPago ?? ""));
                Console.WriteLine("Método de Pago : " + (o.MetodoPago ?? ""));
                Console.WriteLine("Estado O/S     : " + (o.Estado ?? "").ToUpper());
            }
            Console.WriteLine("--------------------------------------------------");
        }
    }
}