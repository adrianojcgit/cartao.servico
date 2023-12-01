using Cartao.Domain.Domains.PropostaContext.Dto;

namespace Cartao.Domain.Domains.PropostaContext.Contract
{
    public interface IProposta
    {
        //Task<List<ClienteDto>> GetAsync();

        //Task<ClienteDto?> GetAsync(string id);

        Task CreateAsync(PropostaBaseDto newAluno);

        //Task UpdateAsync(string id, ClienteDto updatedAluno);

        //Task RemoveAsync(string id);
    }
}
