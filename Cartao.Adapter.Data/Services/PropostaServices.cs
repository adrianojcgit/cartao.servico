using Cartao.Adapter.Data.Model;
using Cartao.Domain.Infra;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Cartao.Adapter.Data.Services
{
    public class PropostaServices
    {
        private readonly IMongoCollection<Cliente> _AlunoCollection;

        public PropostaServices(IOptions<DatabaseSettings> dabaseSettings)
        {
            var mongoClient = new MongoClient(dabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dabaseSettings.Value.DatabaseName);
            _AlunoCollection = mongoDatabase.GetCollection<Cliente>("Cliente");
        }

        public async Task<List<Cliente>> GetAsync() =>
            await _AlunoCollection.Find(_ => true).ToListAsync();

        public async Task<Cliente?> GetAsync(string id) =>
            await _AlunoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Cliente newAluno) =>
            await _AlunoCollection.InsertOneAsync(newAluno);

        public async Task UpdateAsync(string id, Cliente updatedAluno) =>
            await _AlunoCollection.ReplaceOneAsync(x => x.Id == id, updatedAluno);

        public async Task RemoveAsync(string id) => await
                       _AlunoCollection.DeleteOneAsync(x => x.Id == id);
    }
}
