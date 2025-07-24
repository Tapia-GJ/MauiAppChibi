using MauiMySql.Clases;
using MauiMySql.Models;
using System.Collections.ObjectModel;

namespace MauiMySql.Views;

public partial class CarritoView : ContentPage
{
    private ObservableCollection<ProductoCarrito> productos = new();
    private readonly Consultas consultas = new();

    public CarritoView()
    {
        InitializeComponent();
        BindingContext = this;
        CarritoCollectionView.ItemsSource = productos;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            bool recargar = Preferences.Get("recargar_carrito", false);
            IsBusy = true;

            if (productos.Count == 0 || recargar) // Solo carga si no se han cargado ya
            {
                await consultas.InitializeAsync();
                await CargarProductosEnCarrito();
            }
        }
        finally
        {
            Preferences.Set("recargar_carrito", false);
            IsBusy = false;
        }
    }

    private async Task CargarProductosEnCarrito()
    {
        productos.Clear();

        long carritoId = Preferences.Get("carrito_id", 0L);
        if (carritoId == 0)
        {
            await DisplayAlert("Error", "No se encontró un carrito activo", "OK");
            return;
        }

        var lista = await consultas.ObtenerProductosDeCarritoAsync(carritoId);
        foreach (var p in lista)
        {
            if (string.IsNullOrEmpty(p.Producto.ImagenPath))
                p.Producto.ImagenPath = "placeholder.png";

            productos.Add(p);
        }
    }

    private void SumarCantidad_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is ProductoCarrito pc)
        {
            pc.Cantidad++;
            OnPropertyChanged(nameof(Total));
        }
    }

    private void RestarCantidad_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is ProductoCarrito pc && pc.Cantidad > 1)
        {
            pc.Cantidad--;
            OnPropertyChanged(nameof(Total));
        }
    }

    private async void Eliminar_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is ProductoCarrito pc)
        {
            bool confirm = await DisplayAlert("Confirmar", $"¿Eliminar {pc.Producto.Nombre} del carrito?", "Sí", "No");
            if (confirm)
            {
                await consultas.EliminarProductoDeCarritoAsync((int)pc.id);
                productos.Remove(pc);
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    private async void Pagar_Clicked(object sender, EventArgs e)
    {
        double total = productos.Sum(p => p.PrecioTotal);
        await DisplayAlert("Pago", $"Total a pagar: ${total:F2}", "OK");
    }

    public double Total => productos.Sum(p => p.PrecioTotal);
}
