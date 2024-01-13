using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;

namespace avaloniaTest;

public partial class MainWindow : Window
{
    private Program.Bocadillo bocadillo_actual;
    private List<Program.Bocadillo> lista_bocadillos;
    private string ruta;
    private int num_boc_act;
    private byte[] imagenActual;


    public MainWindow()
    {
        InitializeComponent();

    }

    private void CargarBocadilloPantalla(int num)
    {
        int tipopan;
        LblBocadilos.Content = (num_boc_act + 1) + "/" + lista_bocadillos.Count;
        bocadillo_actual = lista_bocadillos[num];
        TxtBocadillo.Text = bocadillo_actual.ToString();
        TxtPrecio.Text = bocadillo_actual.Precio.ToString();
        NumStock.Text = bocadillo_actual.Stock.ToString();
        ChbCaliente.IsEnabled = bocadillo_actual.Calentito;
        switch (bocadillo_actual.TipoPan)
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
        Bitmap image = null;
        if (bocadillo_actual.Imagen != null)
        {
            using (var ms = new MemoryStream(bocadillo_actual.Imagen))
            {
                image = new Bitmap(ms);
            }

        }
        ImgBocadillo.Source = image;
        if (num > 0) { BtnPrev.IsEnabled = true; }
        else { BtnPrev.IsEnabled = false; }
        if (num < lista_bocadillos.Count - 1) { BtnNext.IsEnabled = true; }
        else { BtnNext.IsEnabled = false; }
    }

    private void BtnBocadilloSiguiente(object? sender, RoutedEventArgs e)
    {
        num_boc_act++;
        CargarBocadilloPantalla(num_boc_act);
    }

    private void BtnBocadilloAnterior(object? sender, RoutedEventArgs e)
    {
        num_boc_act--;
        CargarBocadilloPantalla(num_boc_act);
    }

    private void EliminarBocadillo(object? sender, RoutedEventArgs e)
    {

        lista_bocadillos.Remove(bocadillo_actual);
        if (num_boc_act >= lista_bocadillos.Count)
            num_boc_act--;
        LblBocadilos.Content = (num_boc_act + 1) + "/" + lista_bocadillos.Count;
        MostrarSiHay();
    }

    private void BtnCrearBocadillo(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void BtnCargarBocadillo(object? sender, RoutedEventArgs e)
    {
        {
            try
            {
                if (File.Exists(ruta))
                    lista_bocadillos = Program.CargarBocadillos(ruta);
                if (lista_bocadillos.Count > 0)
                {
                    num_boc_act = 0;
                    CargarBocadilloPantalla(num_boc_act);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

    private void BtnGuardarBocadillo(object? sender, RoutedEventArgs e)
    {
        Program.GuardarBocadillos(lista_bocadillos, ruta);
    }

    private void CambiarImagen(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void CbBocadillo_Checked(object? sender, RoutedEventArgs e)
    {

    }

    private void MostrarSiHay()
    {
        if (lista_bocadillos.Count > 0)
        {
            CargarBocadilloPantalla(num_boc_act);
        }
        else
        {
            LimpiarTextos();
            BtnEliminarBocadillo.IsEnabled = false;
            BtnCambiarImagen.IsEnabled = false;
        }
        imagenActual = null;
    }

    private void LimpiarTextos()
    {
        TxtBocadillo.Text = "";
        TxtPrecio.Text = "";
        NumStock.Text = "";
        ChbCaliente.IsChecked = false;
        CbTipoPan.SelectedIndex = 0;
        ImgBocadillo.Source = null;
    }


}