using Cartao.Domain.Domains.PropostaContext.Dto;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
namespace Cartao.Domain.Infra
{
    public sealed class Proposta
    {
        private readonly IConfiguration _configuration;

        public Proposta(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public static PropostaBaseDto ReadXlsProspect(string NomeArquivo, string DirArqClienteOrigem)
        {
            
            var response = new List<PropostaDto>();
            var arquivo = DirArqClienteOrigem + NomeArquivo;

            FileInfo existingFile = new FileInfo(fileName: arquivo);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int colCount = worksheet.Dimension.End.Column;
                int rowCount = worksheet.Dimension.End.Row;

                for (int row = 2; row <= rowCount; row++)
                {
                    var cliente = new PropostaDto
                    {
                        IdHtml = int.Parse(worksheet.Cells[row, 1].Value.ToString()),
                        CodInterno = worksheet.Cells[row, 2].Value.ToString(),
                        CNPJ = worksheet.Cells[row, 4].Value.ToString(),
                        NomeEmpresarial = worksheet.Cells[row, 7].Value.ToString(),
                        NomeFantasia = worksheet.Cells[row, 8].Value.ToString(),
                    };

                    var endereco = new EnderecoDto()
                    {
                        Logradouro = worksheet.Cells[row, 9].Value is null ? string.Empty : worksheet.Cells[row, 9].Value.ToString(),
                        Numero = worksheet.Cells[row, 10].Value is null ? string.Empty : worksheet.Cells[row, 10].Value.ToString(),
                        Complemento = worksheet.Cells[row, 11].Value is null ? string.Empty : worksheet.Cells[row, 11].Value.ToString(),
                        CEP = worksheet.Cells[row, 12] is null ? string.Empty : worksheet.Cells[row, 12].Value.ToString(),
                        Bairro = worksheet.Cells[row, 13].Value is null ? string.Empty : worksheet.Cells[row, 13].Value.ToString(),
                    };
                    var contaBanco = new ContaBancoDto()
                    {
                        Agencia = "",
                        Banco = "",
                        ContaCorrente = "",
                        Municipio = "",
                        UF = ""
                    };
                    cliente.ContasBancos.Add(contaBanco);
                    cliente.Enderecos.Add(endereco);
                    response.Add(cliente);
                }
            }
            var propostas = new PropostaBaseDto(response);
            return propostas;
        }

        //public static List<ClienteDto> ReadXlsAcoes(string NomeArquivo, string DirArqClienteOrigem)
        //{
        //    var response = new List<ClienteDto>();
        //    var arquivo = DirArqClienteOrigem + NomeArquivo;

        //    FileInfo existingFile = new FileInfo(fileName: arquivo);

        //    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        //    using (ExcelPackage package = new ExcelPackage(existingFile))
        //    {
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        //        int colCount = worksheet.Dimension.End.Column;
        //        int rowCount = worksheet.Dimension.End.Row;

        //        for (int row = 2; row < rowCount; row++)
        //        {
        //            var cliente = new ClienteDto
        //            {
        //                IdHtml = int.Parse(worksheet.Cells[row, 1].Value.ToString()),
        //                CodInterno = worksheet.Cells[row, 2].Value.ToString(),
        //                CNPJ = worksheet.Cells[row, 4].Value.ToString(),
        //                NomeEmpresarial = worksheet.Cells[row, 7].Value.ToString(),
        //                NomeFantasia = worksheet.Cells[row, 8].Value.ToString()
        //            };

        //            var endereco = new EnderecoDto()
        //            {
        //                Logradouro = worksheet.Cells[row, 9].Value is null ? string.Empty : worksheet.Cells[row, 9].Value.ToString(),
        //                Numero = worksheet.Cells[row, 10].Value is null ? string.Empty : worksheet.Cells[row, 10].Value.ToString(),
        //                Complemento = worksheet.Cells[row, 11].Value is null ? string.Empty : worksheet.Cells[row, 11].Value.ToString(),
        //                CEP = worksheet.Cells[row, 12] is null ? string.Empty : worksheet.Cells[row, 12].Value.ToString(),
        //                Bairro = worksheet.Cells[row, 13].Value is null ? string.Empty : worksheet.Cells[row, 13].Value.ToString(),
        //            };
        //            cliente.Enderecos.Add(endereco);
        //            response.Add(cliente);
        //        }
        //    }

        //    return response.ToList();
        //}
    }
}