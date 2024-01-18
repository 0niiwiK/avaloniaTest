using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using AvaloniaTest;

namespace avaloniaTest
{
    // Clase principal que representa la ventana principal de la aplicación
    public partial class MainWindow : Window
    {
        // Variables miembro
        private Program.Bocadillo _bocadilloActual;
        public static List<Program.Bocadillo> ListaBocadillos;
        private string _ruta;
        public static int NumBocAct;
        private byte[] _imagenActual;
        private byte[] _streamImagen;
        private MemoryStream _imageMemory;

        // Constructor de la ventana principal
        public MainWindow()
        {
            InitializeComponent();

            // Inicialización de variables
            ListaBocadillos = new List<Program.Bocadillo>();
            _ruta = Program.Init();
            CargarBocadillo();

            // Si hay bocadillos cargados, muestra el primer bocadillo
            if (NumBocAct != -1)
            {
                CargarBocadilloPantalla(NumBocAct);
            }
        }

        // Método para cargar la información de un bocadillo en la pantalla
        private void CargarBocadilloPantalla(int num)
        {
            int tipopan;
            LblBocadilos.Content = (NumBocAct + 1) + "/" + ListaBocadillos.Count;
            _bocadilloActual = ListaBocadillos[num];
            TxtBocadillo.Text = _bocadilloActual.Nombre;
            TxtPrecio.Text = _bocadilloActual.Precio.ToString();
            NumStock.Text = _bocadilloActual.Stock.ToString();
            ChbCaliente.IsVisible = _bocadilloActual.Calentito;
            LblTotal.Content = _bocadilloActual.Precio.ToString();

            // Asigna el tipo de pan al índice correspondiente en el ComboBox
            switch (_bocadilloActual.TipoPan)
            {
                case 'A':
                    tipopan = 0; break;
                case 'G':
                    tipopan = 1; break;
                case 'V':
                    tipopan = 2; break;
                default:
                    tipopan = 0; break;
            }
            CbTipoPan.SelectedIndex = tipopan;

            // Carga la imagen del bocadillo actual en el control de imagen
            Bitmap image = null;
            if (_bocadilloActual.Imagen != null)
            {
                using (var ms = new MemoryStream(_bocadilloActual.Imagen))
                {
                    image = new Bitmap(ms);
                }
            }
            ImgBocadillo.Source = image;

            // Habilita o deshabilita los botones de navegación según la posición en la lista
            if (num > 0) { BtnPrev.IsEnabled = true; }
            else { BtnPrev.IsEnabled = false; }
            if (num < ListaBocadillos.Count - 1) { BtnNext.IsEnabled = true; }
            else { BtnNext.IsEnabled = false; }
        }

        // Método para mostrar el siguiente bocadillo
        private void BtnBocadilloSiguiente(object? sender, RoutedEventArgs e)
        {
            NumBocAct++;
            CargarBocadilloPantalla(NumBocAct);
        }

        // Método para mostrar el bocadillo anterior
        private void BtnBocadilloAnterior(object? sender, RoutedEventArgs e)
        {
            NumBocAct--;
            CargarBocadilloPantalla(NumBocAct);
        }

        // Método para eliminar el bocadillo actual
        private void EliminarBocadillo(object? sender, RoutedEventArgs e)
        {
            ListaBocadillos.Remove(_bocadilloActual);

            // Ajusta la posición actual si es necesario
            if (NumBocAct >= ListaBocadillos.Count)
                NumBocAct--;

            LblBocadilos.Content = (NumBocAct + 1) + "/" + ListaBocadillos.Count;
            MostrarSiHay();
        }

        // Método asincrónico para abrir la ventana de creación de un nuevo bocadillo
        private async void BtnCrearBocadillo(object? sender, RoutedEventArgs e)
        {
            var owner = this;
            var nuevoBocadillo = new NuevoBocadillo();
            await nuevoBocadillo.ShowDialog(this);

            // Si hay bocadillos, carga el último creado
            if (ListaBocadillos != null && ListaBocadillos.Count > 0)
            {
                NumBocAct = ListaBocadillos.Count() - 1;
                CargarBocadilloPantalla(NumBocAct);
            }
        }

        // Método para cargar bocadillos desde el archivo
        private void BtnCargarBocadillo(object? sender, RoutedEventArgs e)
        {
            CargarBocadillo();
        }

        // Método para cargar la lista de bocadillos desde el archivo especificado en la ruta
        private void CargarBocadillo()
        {
            try
            {
                if (File.Exists(_ruta))
                    ListaBocadillos = Program.CargarBocadillos(_ruta);

                // Si hay bocadillos, establece la posición actual al primer elemento
                if (ListaBocadillos != null && ListaBocadillos.Count > 0)
                {
                    NumBocAct = 0;
                }
                else
                {
                    NumBocAct = -1;
                    BtnPrev.IsEnabled = false;
                    BtnNext.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones (por implementar)
            }
        }

        // Método para guardar la lista de bocadillos en el archivo especificado en la ruta
        private void BtnGuardarBocadillo(object? sender, RoutedEventArgs e)
        {
            Program.GuardarBocadillos(ListaBocadillos, _ruta);
        }

        // Método asincrónico para cambiar la imagen del bocadillo actual
        private async void CambiarImagen(object? sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Seleccionar imagen",
                Filters = new List<FileDialogFilter>
                {
                    new()
                    {
                        Name = "Imágenes",
                        Extensions = new List<string> { "png", "jpg", "jpeg", "gif", "bmp" }
                    }
                }
            };

            var selectedFiles = await openFileDialog.ShowAsync(this);

            // Si se selecciona un archivo, carga la imagen y actualiza el bocadillo actual
            if (selectedFiles != null && selectedFiles.Length > 0)
            {
                var imagePath = selectedFiles[0];
                Bitmap bitmap;

                using (var stream = File.OpenRead(imagePath))
                {
                    bitmap = new Bitmap(stream);
                    ImgBocadillo.Source = bitmap;
                }

                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream);
                    _bocadilloActual.Imagen = memoryStream.ToArray();
                }
            }
        }

        // Método para calcular el precio total al marcar o desmarcar la opción "Caliente"
        private void CbBocadillo_Checked(object? sender, RoutedEventArgs e)
        {
            double precio = _bocadilloActual.Precio;
            if ((bool)ChbCaliente.IsChecked)
            {
                precio += 0.25;
            }
            else
            {
                precio = _bocadilloActual.Precio;
            }
            LblTotal.Content = precio.ToString();
        }

        // Método para mostrar la información del bocadillo actual o limpiar la pantalla si no hay bocadillos
        private void MostrarSiHay()
        {
            if (ListaBocadillos.Count > 0)
            {
                CargarBocadilloPantalla(NumBocAct);
            }
            else
            {
                LimpiarTextos();
                BtnEliminarBocadillo.IsEnabled = false;
                BtnCambiarImagen.IsEnabled = false;
            }
            _imagenActual = null;
        }

        // Método para limpiar los campos de texto y deseleccionar opciones al cerrar la ventana principal
        private void LimpiarTextos()
        {
            TxtBocadillo.Text = "";
            TxtPrecio.Text = "";
            NumStock.Text = "";
            ChbCaliente.IsChecked = false;
            CbTipoPan.SelectedIndex = 0;
            ImgBocadillo.Source = null;
        }

        // Manejador de eventos al cerrar la ventana principal
        private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
        {
            // Guarda la lista de bocadillos antes de cerrar la aplicación
            Console.Write("Cerrando...");
            Program.GuardarBocadillos(ListaBocadillos, _ruta);
        }
    }
}
