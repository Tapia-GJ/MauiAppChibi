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
        ErrorFrame.IsVisible = false;

        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MostrarError("Ingresa tu correo y contraseña.");
            return;
        }
        if (!EsCorreoValido(email))
        {
            MostrarError("El correo ingresado no tiene un formato válido.");
            return;
        }

        // NUEVO: recibe tupla con mensaje y estado
        var (exito, mensaje) = await _consultas.IniciarSesionAsync(email, password);

        if (exito)
        {
            var userId = _consultas.ObtenerUsuarioActualId();

            // Crear carrito si no tiene
            var carritoId = await _consultas.ObtenerOCrearCarritoPorUsuarioAsync(userId);
            Preferences.Set("carrito_id", carritoId);
            Preferences.Set("user_id", userId);

            await DisplayAlert("Éxito", mensaje, "OK");
            await Shell.Current.GoToAsync("//home");
        }
        else
        {
            MostrarError(mensaje); // Muestra el mensaje específico
        }
    }

    private async void Registrarse_Clicked(object sender, EventArgs e)
    {
        ErrorFrame.IsVisible = false;

        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MostrarError("Correo y contraseña requeridos.");
            return;
        }
        if (!EsCorreoValido(email))
        {
            MostrarError("El correo ingresado no tiene un formato válido.");
            return;
        }

        // NUEVO: también devuelve mensaje personalizado
        var (exito, mensaje) = await _consultas.RegistrarseAsync(email, password);

        if (exito)
        {
            await DisplayAlert("Registro exitoso", mensaje, "OK");
        }
        else
        {
            MostrarError(mensaje); // Muestra mensaje real (ej. ya registrado)
        }
    }

    private void MostrarError(string mensaje)
    {
        MensajeError.Text = mensaje;
        ErrorFrame.IsVisible = true;
    }

    private bool EsCorreoValido(string correo)
    {
        return !string.IsNullOrWhiteSpace(correo) &&
               System.Text.RegularExpressions.Regex.IsMatch(
                   correo,
                   @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                   System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
}
