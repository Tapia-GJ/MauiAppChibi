using MauiMySql.DTO;
using MauiMySql.Models;
using Supabase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiMySql.Clases
{
    internal class Consultas
    {
        private Supabase.Client _client;

        public Consultas()
        {
            var url = "https://asczbyxadxprsxszicqu.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImFzY3pieXhhZHhwcnN4c3ppY3F1Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDI2ODcwNDYsImV4cCI6MjA1ODI2MzA0Nn0.-DgiaxUO4DQoZF4cblmJvPVT8iEvbnClW9U7Npu4ao4";

            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            _client = new Supabase.Client(url, key, options);
        }

        public async Task InitializeAsync()
        {
            await _client.InitializeAsync();
        }

        public async Task CreateAsync(Producto p)
        {
            await _client.From<Producto>().Insert(p);
        }

        public async Task<List<Producto>> GetAllAsync()
        {
            var response = await _client
                .From<Producto>()
                .Get();

            return response.Models;
        }

        public async Task<Producto> GetByIdAsync(int id)
        {
            var response = await _client
                .From<Producto>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id)
                .Get();

            return response.Models.FirstOrDefault();
        }

        public async Task<List<Producto>> BuscarPorNombreODescripcionAsync(string texto)
        {
            var response1 = await _client
                .From<Producto>()
                .Filter("nombre", Supabase.Postgrest.Constants.Operator.ILike, $"%{texto}%")
                .Get();

            var response2 = await _client
                .From<Producto>()
                .Filter("descripcion", Supabase.Postgrest.Constants.Operator.ILike, $"%{texto}%")
                .Get();

            var resultados = response1.Models
                .Concat(response2.Models)
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .ToList();

            return resultados;
        }

        public async Task UpdateAsync(Producto p)
        {
            await _client.From<Producto>().Update(p);
        }

        public async Task DeleteAsync(int id)
        {
            var producto = new Producto { Id = id };
            await _client.From<Producto>().Delete(producto);
        }
        public Supabase.Client GetClient()
        {
            return _client;
        }
        public async Task<bool> IniciarSesionAsync(string email, string password)
        {
            await _client.InitializeAsync();

            var session = await _client.Auth.SignIn(email, password);

            return session.User != null;
        }
        public async Task<bool> RegistrarseAsync(string email, string password)
        {
            await _client.InitializeAsync();

            var session = await _client.Auth.SignUp(email, password);

            return session.User != null;
        }
        public string ObtenerUsuarioActualId()
        {
            return _client.Auth.CurrentUser?.Id;
        }
        public async Task<long> ObtenerOCrearCarritoPorUsuarioAsync(string userId)
        {
            await _client.InitializeAsync();

            var resultado = await _client
                .From<Carrito>()
                .Filter("cliente_id", Supabase.Postgrest.Constants.Operator.Equals, userId)
                .Get();

            // Si ya existe un carrito, devolver su ID
            if (resultado.Models.Count > 0)
                return resultado.Models[0].Id;

            // Si no existe, crearlo
            var nuevoCarrito = new Carrito
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var insercion = await _client.From<Carrito>().Insert(nuevoCarrito);
            return insercion.Models[0].Id;
        }
        public async Task<List<ProductoCarrito>> ObtenerProductosDeCarritoAsync(long carritoId)
        {
            var items = await _client
                .From<ItemCarrito>()
                .Filter("carrito_id", Supabase.Postgrest.Constants.Operator.Equals, (int)carritoId)
                .Get();

            var lista = new List<ProductoCarrito>();

            foreach (var item in items.Models)
            {
                var producto = await GetByIdAsync((int)item.ProductoId);
                if (producto != null)
                {
                    lista.Add(new ProductoCarrito
                    {
                        id = (int)item.Id,
                        Producto = producto,
                        Cantidad = item.Cantidad
                    });
                }
            }

            return lista;
        }
        public async Task AgregarItemCarritoAsync(ItemCarrito item)
        {
            await _client.From<ItemCarrito>().Insert(item);
        }
        public async Task EliminarProductoDeCarritoAsync(int id)
        {
            var itemProducto = new ItemCarrito { Id = id };
            await _client.From<ItemCarrito>().Delete(itemProducto);
        }




    }
}
