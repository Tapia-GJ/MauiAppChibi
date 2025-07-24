using MauiMySql.Clases;
using MauiMySql.DTO;
using System.Collections.ObjectModel;

namespace MauiMySql.Views;

public partial class AgregarView : ContentPage
{
    private static List<Producto>? _cacheProductos = null;
    ObservableCollection<Producto> productos = new();
    Consultas consultas = new();
    bool datosCargados = false;

    public AgregarView()
    {
        InitializeComponent();
        ProductosCollectionView.ItemsSource = productos;
        EntryBuscar.Completed += EntryBuscar_Completed;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!datosCargados)
        {
            await consultas.InitializeAsync();
            await CargarProductosDesdeSupabaseAsync();
            datosCargados = true;
        }
    }

    private async Task CargarProductosDesdeSupabaseAsync()
    {
        productos.Clear();

        if (_cacheProductos != null)
        {
            foreach (var p in _cacheProductos)
                productos.Add(p);
            return;
        }

        var lista = await consultas.GetAllAsync();
        _cacheProductos = lista;

        foreach (var p in lista)
        {
            if (string.IsNullOrEmpty(p.ImagenPath))
                p.ImagenPath = "placeholder.png";

            productos.Add(p);
        }
    }

    private async void EntryBuscar_Completed(object sender, EventArgs e)
    {
        var texto = EntryBuscar.Text;

        productos.Clear();

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var lista = await consultas.BuscarPorNombreODescripcionAsync(texto);

            foreach (var p in lista)
            {
                if (string.IsNullOrEmpty(p.ImagenPath))
                    p.ImagenPath = "placeholder.png";

                productos.Add(p);
            }
        }
        else
        {
            await CargarProductosDesdeSupabaseAsync();
        }
    }

    private async void AgregarAlCarrito_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Producto producto)
        {
            await consultas.InitializeAsync();

            var carritoId = Preferences.Get("carrito_id", 0L);
            if (carritoId == 0)
            {
                await DisplayAlert("Error", "No se encontró un carrito activo", "OK");
                return;
            }

            var item = new ItemCarrito
            {
                CarritoId = carritoId,
                ProductoId = producto.Id,
                Cantidad = 1,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await DisplayAlert("Carrito", "Producto agregado al carrito ", "OK");
                await consultas.AgregarItemCarritoAsync(item);
                Preferences.Set("recargar_carrito", true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }
    }
}
