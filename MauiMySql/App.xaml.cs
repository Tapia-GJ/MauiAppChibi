namespace MauiMySql
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            window.Created += (_, _) =>
            {
                window.Dispatcher.Dispatch(() =>
                {
                    RedirigirSegunSesion();
                });
            };

            return window;
        }

        private async void RedirigirSegunSesion()
        {
            var userId = Preferences.Get("user_id", null);

            if (string.IsNullOrEmpty(userId))
                await Shell.Current.GoToAsync("//login");
            else
                await Shell.Current.GoToAsync("//carrito");
        }
    }
}
