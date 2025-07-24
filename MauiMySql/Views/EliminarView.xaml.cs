using MauiMySql.Clases;
using MauiMySql.DTO;

namespace MauiMySql.Views;

public partial class EliminarView : ContentPage
{
    Consultas consultas = new Consultas();
    Producto productoActual;

    public EliminarView()
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
            LimpiarCampos();
            return;
        }

        EntryNombre.Text = productoActual.Nombre;
        EntryDescripcion.Text = productoActual.Descripcion;
        EntryStock.Text = productoActual.Stock.ToString();
        EntryPrecio.Text = productoActual.Precio.ToString();
    }

    private async void OnEliminarProductoClicked(object sender, EventArgs e)
    {
        if (productoActual == null)
        {
            await DisplayAlert("Error", "Primero busque un producto.", "OK");
            return;
        }

        var confirm = await DisplayAlert(
            "Confirmar eliminación",
            $"¿Está seguro de eliminar el producto \"{productoActual.Nombre}\"?",
            "Sí", "No");

        if (!confirm)
            return;

        await consultas.DeleteAsync((int)productoActual.Id);

        await DisplayAlert("Éxito", "Producto eliminado correctamente.", "OK");

        LimpiarCampos();
    }

    private void LimpiarCampos()
    {
        EntryBuscarId.Text = string.Empty;
        EntryNombre.Text = string.Empty;
        EntryDescripcion.Text = string.Empty;
        EntryStock.Text = string.Empty;
        EntryPrecio.Text = string.Empty;
        productoActual = null;
    }
}