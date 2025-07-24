using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace MauiMySql.Services
{
    public class MercadoPagoService
    {
        private readonly HttpClient _httpClient;

        public MercadoPagoService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.mercadopago.com/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "APP_USR-7443210713540099-070815-7aff9e9b4eea2620cdfd147eb2381dfe-2541447987");
            // ⚠ Reemplaza TU_ACCESS_TOKEN por tu Access Token TEST-...
        }

        public async Task<string?> CrearPreferencia(decimal monto, string descripcion)
        {
            var body = new
            {
                items = new[]
                {
            new {
                title = descripcion,
                quantity = 1,
                currency_id = "MXN",
                unit_price = monto
            }
        },
                back_urls = new
                {
                    success = "https://tusitio.com/success",
                    failure = "https://tusitio.com/failure"
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("checkout/preferences", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(jsonResponse);
                var initPoint = doc.RootElement.GetProperty("init_point").GetString();
                return initPoint;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("🚫 ERROR MERCADO PAGO:");
                Console.WriteLine(error);

                // También puedes mostrarlo en pantalla si quieres:
                await Shell.Current.DisplayAlert("Error Mercado Pago", error, "OK");

                return null;
            }
        }

    }
}
