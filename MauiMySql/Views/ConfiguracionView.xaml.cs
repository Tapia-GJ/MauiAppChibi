using MauiMySql.Clases;

namespace MauiMySql.Views;

public partial class ConfiguracionView : ContentPage
{
    public ConfiguracionView()
    {
        InitializeComponent();
    }

    private async void CerrarSesion_Clicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Cerrar sesión", "¿Seguro que deseas salir?", "Sí", "Cancelar");

        if (confirm)
        {
            var consultas = new Consultas();
            var client = consultas.GetClient();
            await client.Auth.SignOut();

            Preferences.Remove("user_id");
            Preferences.Remove("carrito_id");

            await Shell.Current.GoToAsync("//login");
        }
    }
}
