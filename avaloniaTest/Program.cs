using Avalonia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace avaloniaTest;

class Program
{
    public static List<Bocadillo> lista_bocadillos { get; private set; }

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>


    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    public static string Init()
    {
        string ruta = ObtenerRuta("databank.data");

        return ruta;
    }

    public static string ObtenerRuta(string archivo)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), archivo);

    }

    public static List<Bocadillo> CargarBocadillos(string ruta)
    {
        List<Bocadillo> bocadillos = new List<Bocadillo>();

        using (FileStream fs = new FileStream(ruta, FileMode.Open))
        using (BinaryReader reader = new BinaryReader(fs))
        {
            int count = reader.ReadInt32(); // Lee la cantidad de bocadillos

            for (int i = 0; i < count; i++)
            {
                string nombre = LeerStringConTamanio(reader);
                int stock = reader.ReadInt32();
                double precio = reader.ReadDouble();
                char tipoPan = reader.ReadChar();
                bool calentito = reader.ReadBoolean();

                int imagenSize = reader.ReadInt32();
                byte[] imagen = reader.ReadBytes(imagenSize);

                Bocadillo bocadillo = new Bocadillo(nombre, stock, precio, tipoPan, calentito, imagen);
                bocadillos.Add(bocadillo);
            }
        }

        return bocadillos;
    }

    public static void GuardarBocadillos(List<Bocadillo> lista, string ruta)
    {
        using (FileStream fs = new FileStream(ruta, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            writer.Write(lista.Count); // Escribe la cantidad de bocadillos

            foreach (var bocadillo in lista)
            {
                EscribirStringConTamanio(writer, bocadillo.Nombre);
                writer.Write(bocadillo.Stock);
                writer.Write(bocadillo.Precio);
                writer.Write(bocadillo.TipoPan);
                writer.Write(bocadillo.Calentito);

                writer.Write(bocadillo.Imagen.Length);
                writer.Write(bocadillo.Imagen);
            }
        }
    }

    public static List<Bocadillo> CargarBocadillosRetro(string ruta)
    {
        List<Bocadillo> bocadillos = new List<Bocadillo>();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(ruta, FileMode.Open);
        bocadillos = (List<Bocadillo>)bf.Deserialize(fs);
        fs.Close();
        return bocadillos;
    }

    public static void GuardarBocadillosRetro(List<Bocadillo> lista, string ruta)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(ruta, FileMode.Create);
        bf.Serialize(fs, lista);
        fs.Flush();
        fs.Close();
    }

    private static string LeerStringConTamanio(BinaryReader reader)
    {
        int length = reader.ReadInt32();
        byte[] stringBytes = reader.ReadBytes(length);
        return Encoding.UTF8.GetString(stringBytes);
    }

    private static void EscribirStringConTamanio(BinaryWriter writer, string value)
    {
        byte[] stringBytes = Encoding.UTF8.GetBytes(value);
        writer.Write(stringBytes.Length);
        writer.Write(stringBytes);
    }

    [Serializable]
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