using System;

namespace LavanderiaOrquideas
{
    public static class Utilidades
    {
        // Validación: Nombre no vacío y sin números/caracteres especiales
        public static string LeerNombreValido(string mensaje)
        {
            string nombre;
            bool esValido;
            do
            {
                Console.Write(mensaje);
                nombre = Console.ReadLine()?.Trim();
                esValido = !string.IsNullOrEmpty(nombre);

                if (esValido)
                {
                    foreach (char c in nombre)
                    {
                        if (!char.IsLetter(c) && c != ' ')
                        {
                            esValido = false;
                            break;
                        }
                    }
                }

                if (!esValido)
                {
                    Console.WriteLine("Error: Ingrese un nombre válido (solo letras y espacios).");
                }
            } while (!esValido);

            return nombre;
        }

        // Validación: Teléfono de exactamente 9 dígitos
        public static string LeerTelefonoValido(string mensaje)
        {
            string telefono;
            bool esValido;
            do
            {
                Console.Write(mensaje);
                telefono = Console.ReadLine()?.Trim();
                esValido = telefono != null && telefono.Length == 9 && long.TryParse(telefono, out _);

                if (!esValido)
                {
                    Console.WriteLine("Error: El teléfono debe tener exactamente 9 dígitos.");
                }
            } while (!esValido);

            return telefono;
        }

        // Validación: Número decimal positivo (> 0)
        public static decimal LeerDecimalPositivo(string mensaje)
        {
            decimal valor;
            while (true)
            {
                Console.Write(mensaje);
                if (decimal.TryParse(Console.ReadLine(), out valor) && valor > 0)
                {
                    return valor;
                }
                Console.WriteLine("Error: Ingrese un monto numérico mayor a 0.");
            }
        }

        // Validación: Entero positivo (> 0)
        public static int LeerEnteroPositivo(string mensaje)
        {
            int valor;
            while (true)
            {
                Console.Write(mensaje);
                if (int.TryParse(Console.ReadLine(), out valor) && valor > 0)
                {
                    return valor;
                }
                Console.WriteLine("Error: Ingrese una cantidad entera mayor a 0.");
            }
        }

        // Formato para mostrar fecha
        public static string FormatearFecha(DateTime fecha)
        {
            return fecha.ToString("dd/MM/yyyy");
        }
    }
}
