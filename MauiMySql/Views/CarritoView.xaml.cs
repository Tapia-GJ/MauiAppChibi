using MauiMySql.Clases;
using MauiMySql.Models;
using System.Collections.ObjectModel;
using System.Text;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace MauiMySql.Views;

public partial class CarritoView : ContentPage
{
    public ObservableCollection<ProductoCarrito> productos { get; set; } = new();
    private readonly Consultas consultas = new();

    public CarritoView()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            bool recargar = Preferences.Get("recargar_carrito", false);
            IsBusy = true;

            // Asegurar que siempre exista carrito
            long carritoId = Preferences.Get("carrito_id", 0L);
            if (carritoId == 0)
            {
                var userId = consultas.ObtenerUsuarioActualId();
                carritoId = await consultas.ObtenerOCrearCarritoPorUsuarioAsync(userId);
                Preferences.Set("carrito_id", carritoId);
            }

            if (productos.Count == 0 || recargar)
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
        OnPropertyChanged(nameof(Total));
    }

    private async void SumarCantidad_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ProductoCarrito pc)
        {
            pc.Cantidad++;
            await consultas.ActualizarCantidadItemCarritoAsync(pc.id, pc.Cantidad);

            // Actualizar el Label
            if (button.Parent is StackLayout parent)
            {
                var label = parent.Children.OfType<Label>().FirstOrDefault();
                if (label != null)
                    label.Text = pc.Cantidad.ToString();
            }

            OnPropertyChanged(nameof(Total));
        }
    }

    private async void RestarCantidad_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ProductoCarrito pc && pc.Cantidad > 1)
        {
            pc.Cantidad--;
            await consultas.ActualizarCantidadItemCarritoAsync(pc.id, pc.Cantidad);

            if (button.Parent is StackLayout parent)
            {
                var label = parent.Children.OfType<Label>().FirstOrDefault();
                if (label != null)
                    label.Text = pc.Cantidad.ToString();
            }

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
        var servicioMP = new MauiMySql.Services.MercadoPagoService();

        // Usa la propiedad Total que ya actualizas cuando cambia la cantidad
        decimal montoTotal = this.Total;  // <-- Aquí usas tu propiedad ya calculada

        string descripcion = "Pago de carrito";

        string? urlPago = await servicioMP.CrearPreferencia(montoTotal, descripcion);

        if (!string.IsNullOrEmpty(urlPago))
        {
            await Browser.OpenAsync(urlPago, BrowserLaunchMode.SystemPreferred);
        }
        else
        {
            await DisplayAlert("Error", "No se pudo crear la preferencia de pago", "OK");
        }
    }



    public decimal Total => productos.Sum(p => (decimal)p.PrecioTotal);

    //public double Total => productos.Sum(p => p.PrecioTotal);
}
