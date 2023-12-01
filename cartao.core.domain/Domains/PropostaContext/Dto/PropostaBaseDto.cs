using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Cartao.Domain.Domains.PropostaContext.Dto
{
    public class PropostaBaseDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<PropostaDto> Propostas { get; set; }
        public PropostaBaseDto(List<PropostaDto> propostas)
        {
            Propostas = propostas;
        }
        
    }
}
