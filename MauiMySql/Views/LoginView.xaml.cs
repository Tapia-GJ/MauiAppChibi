using MauiMySql.Clases;

namespace MauiMySql.Views;

public partial class LoginView : ContentPage
{
    private Consultas _consultas;

    public LoginView()
    {
        InitializeComponent();
        _consultas = new Consultas();
    }

    private async void IniciarSesion_Clicked(object sender, EventArgs e)
    {
        MensajeError.IsVisible = false;

        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MostrarError("Ingresa tu correo y contraseña.");
            return;
        }

        var exito = await _consultas.IniciarSesionAsync(email, password);

        if (exito)
        {
            var userId = _consultas.ObtenerUsuarioActualId();

            // Crear carrito si no tiene
            var carritoId = await _consultas.ObtenerOCrearCarritoPorUsuarioAsync(userId);
            Preferences.Set("carrito_id", carritoId);
            Preferences.Set("user_id", userId);

            await DisplayAlert("Éxito", $"Bienvenido {email}", "OK");
            await Shell.Current.GoToAsync("//home");
        }
        else
        {
            MostrarError("Correo o contraseña incorrectos.");
        }
    }

    private async void Registrarse_Clicked(object sender, EventArgs e)
    {
        MensajeError.IsVisible = false;

        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MostrarError("Correo y contraseña requeridos.");
            return;
        }

        var exito = await _consultas.RegistrarseAsync(email, password);

        if (exito)
        {
            await DisplayAlert("Registro exitoso", "Ahora puedes iniciar sesión", "OK");
        }
        else
        {
            MostrarError("No se pudo registrar. Verifica el correo.");
        }
    }

    private void MostrarError(string mensaje)
    {
        MensajeError.Text = mensaje;
        MensajeError.IsVisible = true;
    }
}
