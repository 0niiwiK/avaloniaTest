using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using AvaloniaTest;

namespace avaloniaTest
{
    // Clase que representa la ventana para la creación o edición de un bocadillo
    public partial class NuevoBocadillo : Window
    {
        // Lista de bocadillos y variables relacionadas
        public List<Program.Bocadillo> Lista;
        public int NumBocAct;
        private byte[] _imagenActual;
        private Program.Bocadillo _bocadilloActual;

        // Constructor de la clase
        public NuevoBocadillo()
        {
            InitializeComponent();
            Lista = MainWindow.ListaBocadillos;
            NumBocAct = MainWindow.NumBocAct;
            // Si hay elementos en la lista de bocadillos, carga el bocadillo actual en pantalla
            if (Lista != null && Lista.Count > 0)
            {
                CargarBocadilloPantalla();
            }
        }

        // Método para cargar la información del bocadillo actual en pantalla
        private void CargarBocadilloPantalla()
        {
            int tipopan;
            _bocadilloActual = Lista[NumBocAct];
            TxtBocadillo.Text = _bocadilloActual.Nombre;
            TxtPrecio.Text = _bocadilloActual.Precio.ToString();
            NumStock.Text = _bocadilloActual.Stock.ToString();
            ChbCaliente.IsChecked = _bocadilloActual.Calentito;
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
            _imagenActual = _bocadilloActual.Imagen;
        }

        // Método para guardar la información del bocadillo
        private void GuardarBocadillo(object? sender, RoutedEventArgs e)
        {
            char tipopan;
            // Asigna el tipo de pan según el índice seleccionado en el ComboBox
            switch (CbTipoPan.SelectedIndex)
            {
                case 0:
                    tipopan = 'A'; break;
                case 1:
                    tipopan = 'G'; break;
                case 2:
                    tipopan = 'V'; break;
                default:
                    tipopan = 'A'; break;
            }

            try
            {
                // Crea un nuevo objeto Bocadillo con la información ingresada
                Program.Bocadillo nuevo_boc = new Program.Bocadillo(TxtBocadillo.Text, (int)NumStock.Value,
                    double.Parse(TxtPrecio.Text), tipopan, (Boolean)ChbCaliente.IsChecked);
                Lista.Add(nuevo_boc);
                // Si hay una imagen actual, la asigna al nuevo bocadillo
                if (_imagenActual != null)
                {
                    nuevo_boc.Imagen = new byte[_imagenActual.Length];
                    _imagenActual.CopyTo(nuevo_boc.Imagen, 0);
                }
                Close();
            }
            catch (Exception ex)
            {
                LblMensaje.Content = "Error";
                Mensaje.Content = "Faltan datos o son incorrectos";
            }
        }

        // Método para descartar la ventana de creación/edición de bocadillo
        private void DescartarBocadillo(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        // Método asincrónico para agregar una imagen al bocadillo
        private async void AgregarImagen(object? sender, RoutedEventArgs e)
        {
            // Abre el diálogo de selección de archivos para imágenes
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

            // Espera la selección de archivos y obtiene la ruta del primer archivo seleccionado
            var selectedFiles = await openFileDialog.ShowAsync(this);

            if (selectedFiles != null && selectedFiles.Length > 0)
            {
                var imagePath = selectedFiles[0];
                Bitmap bitmap;

                // Lee el archivo de imagen y lo asigna al control de imagen
                using (var stream = File.OpenRead(imagePath))
                {
                    bitmap = new Bitmap(stream);
                    ImgBocadillo.Source = bitmap;
                }

                // Convierte la imagen a un array de bytes y lo asigna al bocadillo actual
                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream);
                    _bocadilloActual.Imagen = memoryStream.ToArray();
                }
                _imagenActual = _bocadilloActual.Imagen;
            }
        }
    }
}
