using Avalonia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using avaloniaTest;

namespace AvaloniaTest
{
    public class Program
    {

        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Configura y construye la aplicación Avalonia.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();

        // Inicializa la aplicación y retorna la ruta del archivo de datos.
        public static string Init()
        {
            string ruta = ObtenerRuta("databank.data");
            return ruta;
        }

        // Obtiene la ruta completa del archivo especificado.
        public static string ObtenerRuta(string archivo)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), archivo);
        }

        // Carga la lista de bocadillos desde el archivo especificado.
        public static List<Bocadillo> CargarBocadillos(string ruta)
        {
            List<Bocadillo> bocadillos = new List<Bocadillo>();

            using (FileStream fs = new FileStream(ruta, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                int count = reader.ReadInt32(); // Lee la cantidad de bocadillos

                for (int i = 0; i < count; i++)
                {
                    Bocadillo bocadillo;
                    string nombre = LeerStringConTamanio(reader);
                    int stock = reader.ReadInt32();
                    double precio = reader.ReadDouble();
                    char tipoPan = reader.ReadChar();
                    bool calentito = reader.ReadBoolean();
                    bool hayImagen = reader.ReadBoolean();

                    if (hayImagen)
                    {
                        int imagenSize = reader.ReadInt32();
                        byte[] imagen = reader.ReadBytes(imagenSize);
                        bocadillo = new Bocadillo(nombre, stock, precio, tipoPan, calentito, imagen);
                    }
                    else
                    {
                        bocadillo = new Bocadillo(nombre, stock, precio, tipoPan, calentito);
                    }

                    bocadillos.Add(bocadillo);
                }
            }

            return bocadillos;
        }

        // Guarda la lista de bocadillos en el archivo especificado.
        public static void GuardarBocadillos(List<Bocadillo> lista, string ruta)
        {
            using (FileStream fs = new FileStream(ruta, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // Escribe la cantidad de bocadillos
                writer.Write(lista.Count);

                foreach (var bocadillo in lista)
                {
                    EscribirStringConTamanio(writer, bocadillo.Nombre);
                    writer.Write(bocadillo.Stock);
                    writer.Write(bocadillo.Precio);
                    writer.Write(bocadillo.TipoPan);
                    writer.Write(bocadillo.Calentito);
                    bool hayImagen = bocadillo.Imagen != null;
                    writer.Write(hayImagen);

                    if (bocadillo.Imagen != null)
                    {
                        writer.Write(bocadillo.Imagen.Length);
                        writer.Write(bocadillo.Imagen);
                    }
                }
            }
        }

        // Lee una cadena de caracteres con su longitud desde el lector binario.
        private static string LeerStringConTamanio(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            byte[] stringBytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(stringBytes);
        }

        // Escribe una cadena de caracteres junto con su longitud en el escritor binario.
        private static void EscribirStringConTamanio(BinaryWriter writer, string value)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(value);
            writer.Write(stringBytes.Length);
            writer.Write(stringBytes);
        }

        public class Bocadillo
        {
            public Bocadillo(string nombre, int stock, double precio, char tipoPan, bool calentito, byte[] imagen)
            {
                Nombre = nombre;
                Stock = stock;
                Precio = precio;
                TipoPan = tipoPan;
                Calentito = calentito;
                Imagen = imagen;
            }

            public Bocadillo(string nombre, int stock, double precio, char tipoPan, bool calentito)
            {
                Nombre = nombre;
                Stock = stock;
                Precio = precio;
                TipoPan = tipoPan;
                Calentito = calentito;
            }

            public string Nombre { get; set; }
            public int Stock { get; set; }
            public double Precio { get; set; }
            public char TipoPan { get; set; }
            public bool Calentito { get; set; }
            public byte[] Imagen { get; set; }

            public override string ToString()
            {
                return "Nombre: " + Nombre + " Stock: " + Stock + " Precio: " + Precio + " TipoPan: " + TipoPan + " Calentito: " + Calentito;
            }
        }
    }
}
