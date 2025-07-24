using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MauiMySql.DTO
{
    [Table("carritos")]
    public class Carrito : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("cliente_id")]
        public string UserId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
