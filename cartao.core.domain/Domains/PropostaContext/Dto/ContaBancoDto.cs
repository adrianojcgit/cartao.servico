using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartao.Domain.Domains.PropostaContext.Dto
{
    public class ContaBancoDto
    {
        public string ContaCorrente { get; set; }
        public string Agencia { get; set; }
        public string Banco { get; set; }
        public string UF { get; set; }
        public string Municipio { get; set; }
    }
}
