using MauiMySql.Clases;
using MauiMySql.DTO;
using System.Collections.ObjectModel;

namespace MauiMySql.Views;

public partial class BuscarView : ContentPage
{
    ObservableCollection<Producto> productos = new ObservableCollection<Producto>();
    Consultas consultas = new Consultas();

    public BuscarView()
    {
        InitializeComponent();
        ResultadosCollectionView.ItemsSource = productos;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await consultas.InitializeAsync();
    }

    private async void OnBuscarClicked(object sender, EventArgs e)
    {
        var texto = EntryBuscar.Text?.Trim();

        if (string.IsNullOrEmpty(texto))
        {
            await DisplayAlert("Error", "Ingrese texto para buscar.", "OK");
            return;
        }

        productos.Clear();

        var lista = await consultas.BuscarPorNombreODescripcionAsync(texto);
        
        if(lista == null || lista.Count == 0)
        {
            await DisplayAlert("Resultado", "No se encontraron productos.", "OK");
            return;
        }

        foreach (var p in lista)
        {
            productos.Add(p);
        }
    }
}