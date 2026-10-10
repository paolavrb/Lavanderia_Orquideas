using System;

namespace LavanderiaOrquideas
{
    class Program
    {
        static OrdenService service = new OrdenService();

        static void Main(string[] args)
        {
            // Cargar datos previos si existen
            service.CargarOrdenes(Almacenamiento.CargarArchivo());

            bool salir = false;
            while (!salir)
            {
                Console.WriteLine("\n=== SISTEMA DE GESTIÓN - LAVANDERÍA ORQUÍDEAS ===");
                Console.WriteLine("1. Registrar nueva orden");
                Console.WriteLine("2. Buscar por cliente");
                Console.WriteLine("3. Buscar por código de orden");
                Console.WriteLine("4. Actualizar estado (Entregar)");
                Console.WriteLine("5. Ordenar por N.° de orden (Algoritmo Burbuja)");
                Console.WriteLine("6. Listar todas las órdenes");
                Console.WriteLine("7. Guardar en archivo local");
                Console.WriteLine("8. Ejecutar Script Demo de Prueba (O/S 106827 - Datos Reales)");
                Console.WriteLine("9. Salir");
                Console.Write("Seleccione una opción: ");

                string opc = Console.ReadLine();
                switch (opc)
                {
                    case "1":
                        service.RegistrarNuevaOrdenInteractiva();
                        break;
                    case "2":
                        Console.Write("Ingrese nombre a buscar: ");
                        service.BuscarPorCliente(Console.ReadLine());
                        break;
                    case "3":
                        Console.Write("Ingrese código de orden: ");
                        if (int.TryParse(Console.ReadLine(), out int cod)) service.BuscarPorCodigo(cod);
                        break;
                    case "4":
                        Console.Write("Ingrese código de orden a entregar: ");
                        if (int.TryParse(Console.ReadLine(), out int codEntregar))
                        {
                            service.ActualizarEstado(codEntregar, "entregado");
                        }
                        break;
                    case "5":
                        service.OrdenarPorBurbuja();
                        break;
                    case "6":
                        service.ListarTodas();
                        break;
                    case "7":
                        Almacenamiento.GuardarArchivo(service.ObtenerTodas());
                        break;
                    case "8":
                        EjecutarDemoReal();
                        break;
                    case "9":
                        salir = true;
                        Console.WriteLine("Saliendo del sistema...");
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
        }

        // Script de prueba con los datos reales observados en el Informe (O/S N.° 106827)
        static void EjecutarDemoReal()
        {
            Console.Clear();
            Console.WriteLine("=== EJECUTANDO DEMO DE PRUEBA (DATOS REALES O/S 106827) ===");

            Orden ordenReal = new Orden
            {
                NumeroOrden = 106827,
                Cliente = "Milagros Panduro Piedra",
                Telefono = "966880335",
                Direccion = "Av. San Felipe 687, Dpto. 803 B, Jesús María",
                FechaRecepcion = new DateTime(2026, 9, 5),
                FechaEntrega = new DateTime(2026, 9, 12),
                ACuenta = 0.00m,
                Estado = "pendiente"
            };

            // Detalle de ítems recibidos según la orden de servicio observada
            decimal p1 = 20.00m; // Zapatilla Palo Rosa Puma
            decimal p2 = 20.00m; // Mochila negro Kipling
            decimal p3 = 20.00m; // Protector colchón 2 plz blanco
            decimal p4 = 14.00m; // Manta delgada beige
            decimal p5 = 27.00m; // Cubrecama 2 plz + 2 fundas

            ordenReal.DetallePrendas = "1 Zapatilla Palo Rosa [Normal] | 1 Mochila negro [Normal] | 1 Protector colchón [Normal] | 1 Manta delgada [Normal] | 1 Cubrecama 2 plz [Normal]";
            ordenReal.Total = p1 + p2 + p3 + p4 + p5; // Suma automática = S/ 101.00

            // 1. Registro
            service.RegistrarOrdenDirecta(ordenReal);
            Console.WriteLine("\n[Paso 1] Orden O/S 106827 registrada en el sistema.");
            service.ImprimirTicket(ordenReal);

            // 2. Validación de Total
            Console.WriteLine($"[Paso 2] Validación de Monto Total Calculado: S/ {ordenReal.Total:F2} (Coincide con la O/S real: S/ 101.00)");

            // 3. Cambio de Estado
            Console.WriteLine("\n[Paso 3] Cambiando estado de 'pendiente' a 'entregado'...");
            service.ActualizarEstado(106827, "entregado");

            // 4. Persistencia
            Almacenamiento.GuardarArchivo(service.ObtenerTodas());
            Console.WriteLine("\n=== PRUEBA DEMO FINALIZADA CON ÉXITO ===");
        }
    }
}
