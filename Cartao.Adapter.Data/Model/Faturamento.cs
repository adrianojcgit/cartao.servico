using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cartao.Adapter.Data.Model
{
    public class Faturamento
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int Id { get; set; }
        public string CodGuid { get; set; }
        public int Ano { get; set; }
        public int Mes { get; set; }
        public int ClienteId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [DataType(DataType.Currency)]
        public decimal? Valor { get; set; }
        public Cliente Cliente { get; set; }
    }
}
