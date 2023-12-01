using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cartao.Adapter.Data.Model
{
    public class Cliente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int IdHtml { get; set; }
        public string? CodGuid { get; set; }
        public string? CodInterno { get; set; }
        public string? ClienteOrigem { get; set; }
        public string? CnpjParametro { get; set; }
        public string? CnpjConsultado { get; set; }
        public string? CnpjNumInscricao { get; set; }
        public string? NomeEmpresarial { get; set; }
        public string? NomeFantasia { get; set; }
        public DateTime DataImportacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public string? PorteEmpresa { get; set; }
        public decimal? FatBrutoAnual { get; set; }
        public bool Ativo { get; set; }
        public string? Logradouro { get; set; }
        public string? Bairro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? CEP { get; set; }
        public List<Faturamento> Faturamentos { get; set; }
    }
}
