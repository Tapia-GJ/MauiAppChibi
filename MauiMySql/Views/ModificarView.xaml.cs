using MauiMySql.Clases;
using MauiMySql.DTO;

namespace MauiMySql.Views;

public partial class ModificarView : ContentPage
{
    Consultas consultas = new Consultas();
    Producto productoActual;

    public ModificarView()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await consultas.InitializeAsync();
    }

    private async void OnBuscarProductoClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(EntryBuscarId.Text, out int id))
        {
            await DisplayAlert("Error", "Ingrese un ID válido.", "OK");
            return;
        }

        productoActual = await consultas.GetByIdAsync(id);

        if (productoActual == null)
        {
            await DisplayAlert("Error", "Producto no encontrado.", "OK");
            return;
        }

        EntryNombre.Text = productoActual.Nombre;
        EntryDescripcion.Text = productoActual.Descripcion;
        EntryStock.Text = productoActual.Stock.ToString();
        EntryPrecio.Text = productoActual.Precio.ToString();
    }

    private async void OnActualizarProductoClicked(object sender, EventArgs e)
    {
        if (productoActual == null)
        {
            await DisplayAlert("Error", "Primero busque un producto.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(EntryNombre.Text) ||
            string.IsNullOrWhiteSpace(EntryDescripcion.Text) ||
            string.IsNullOrWhiteSpace(EntryStock.Text) ||
            string.IsNullOrWhiteSpace(EntryPrecio.Text))
        {
            await DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
            return;
        }

        if (!int.TryParse(EntryStock.Text, out int stock))
        {
            await DisplayAlert("Error", "Stock debe ser un número entero.", "OK");
            return;
        }

        if (!double.TryParse(EntryPrecio.Text, out double precio))
        {
            await DisplayAlert("Error", "Precio debe ser un número decimal.", "OK");
            return;
        }

        productoActual.Nombre = EntryNombre.Text;
        productoActual.Descripcion = EntryDescripcion.Text;
        productoActual.Stock = stock;
        productoActual.Precio = precio;

        await consultas.UpdateAsync(productoActual);

        await DisplayAlert("Éxito", "Producto actualizado correctamente.", "OK");

        // Limpiar campos
        EntryBuscarId.Text = string.Empty;
        EntryNombre.Text = string.Empty;
        EntryDescripcion.Text = string.Empty;
        EntryStock.Text = string.Empty;
        EntryPrecio.Text = string.Empty;
        productoActual = null;
    }
}