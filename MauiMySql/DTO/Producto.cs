using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMySql.DTO
{
    [Table("productos")]
    public class Producto : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("nombre_en_URL")]
        public string NombreEnUrl { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("categoria_id")]
        public long? CategoriaId { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("imagen_path")]
        public string ImagenPath { get; set; }

        [Column("stock")]
        public long Stock { get; set; }

        [Column("precio")]
        public double Precio { get; set; }
    }
}
