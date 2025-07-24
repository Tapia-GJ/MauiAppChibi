using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace MauiMySql.DTO
{
    [Table("items_carrito")]
    public class ItemCarrito : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("carrito_id")]
        public long CarritoId { get; set; }

        [Column("producto_id")]
        public long ProductoId { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
