using Cartao.Domain.Domains.PropostaContext.Contract;
using Cartao.Domain.Domains.PropostaContext.Dto;
using Cartao.Domain.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Cartao.Domain.Domains.PropostaContext.Repositories;

namespace Cartao.Domain.Domains.PropostaContext.Services
{
    public class PropostaServices: IProposta
    {
        private readonly IPropostaRepository _propostaRepository;
        public PropostaServices(IPropostaRepository propostaRepository)
        {
            _propostaRepository = propostaRepository;
        }

        public void Adicionar(PropostaBaseDto propostaBase)
        {
            _propostaRepository.Adicionar(propostaBase);
        }

        //public async Task<List<ClienteDto>> GetAsync() =>
        //    await _ClienteDtoCollection.Find(_ => true).ToListAsync();

        //public async Task<ClienteDto?> GetAsync(string id) =>
        //    await _ClienteDtoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        //public async Task CreateAsync(PropostaBaseDto propostaDto)
        //{
        //    await _propostaDtoCollection.InsertOneAsync(propostaDto);
        //}
        //public async Task UpdateAsync(string id, ClienteDto updatedAluno) =>
        //    await _ClienteDtoCollection.ReplaceOneAsync(x => x.Id == id, updatedAluno);

        //public async Task RemoveAsync(string id) => await
        //               _ClienteDtoCollection.DeleteOneAsync(x => x.Id == id);
    }
}
