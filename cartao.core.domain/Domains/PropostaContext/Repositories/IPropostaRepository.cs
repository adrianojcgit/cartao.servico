using Cartao.Domain.Domains.PropostaContext.Dto;

namespace Cartao.Domain.Domains.PropostaContext.Repositories
{
    public interface IPropostaRepository
    {
        void Adicionar(PropostaBaseDto propostaBaseDto);
    }
}
