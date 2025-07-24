using MauiMySql.DTO;

namespace MauiMySql.Models
{
    public class ProductoCarrito
    {
        public int? id { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public double PrecioTotal => Producto.Precio * Cantidad;
        public string Imagen => Producto.ImagenPath;
        public string Nombre => Producto.Nombre;
    }
}
