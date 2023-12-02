using Cartao.Domain.Domains.PropostaContext.Dto;
using Cartao.Domain.Infra;
using MongoDB.Driver;

namespace Cartao.Domain.Domains.PropostaContext.Repositories
{
    public class PropostaRepository : IPropostaRepository
    {
        private readonly IMongoCollection<PropostaBaseDto> _propostaDtoCollection;
        public PropostaRepository(IDataBaseConfig databaseConfig)
        {
            var client = new MongoClient(databaseConfig.ConnectionString);
            var database = client.GetDatabase(databaseConfig.DataBaseName);
            _propostaDtoCollection = database.GetCollection<PropostaBaseDto>("Propostas");
        }
        public void Adicionar(PropostaBaseDto propostaDto)
        {
             _propostaDtoCollection.InsertOneAsync(propostaDto);
        }
    }
}
