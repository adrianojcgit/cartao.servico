﻿using System;
using System.Collections.Generic;
using System.IO;

public sealed class Proposta
{
    private readonly IConfiguration _configuration;

    public Proposta(IConfiguration configuration)
    {
        _configuration = configuration;

    }

    public static List<Cliente> ReadXlsProspect(string NomeArquivo, string DirArqClienteOrigem, string DirArqClienteDestino)
    {
        var response = new List<Cliente>();
        var arquivo = DirArqClienteOrigem + NomeArquivo;

        FileInfo existingFile = new FileInfo(fileName: arquivo);

        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        using (ExcelPackage package = new ExcelPackage(existingFile))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int colCount = worksheet.Dimension.End.Column;
            int rowCount = worksheet.Dimension.End.Row;

            for (int row = 2; row < rowCount; row++)
            {
                var cliente = new Cliente
                {
                    IdHtml = int.Parse(worksheet.Cells[row, 1].Value.ToString()),
                    CodInterno = worksheet.Cells[row, 2].Value.ToString(),
                    CnpjParametro = worksheet.Cells[row, 4].Value.ToString(),
                    CnpjConsultado = worksheet.Cells[row, 5].Value.ToString(),
                    CnpjNumInscricao = worksheet.Cells[row, 6].Value.ToString(),
                    NomeEmpresarial = worksheet.Cells[row, 7].Value.ToString(),
                    NomeFantasia = worksheet.Cells[row, 8].Value.ToString(),

                    Logradouro = worksheet.Cells[row, 9].Value is null ? string.Empty : worksheet.Cells[row, 9].Value.ToString(),
                    Numero = worksheet.Cells[row, 10].Value is null ? string.Empty : worksheet.Cells[row, 10].Value.ToString(),
                    Complemento = worksheet.Cells[row, 11].Value is null ? string.Empty : worksheet.Cells[row, 11].Value.ToString(),
                    CEP = worksheet.Cells[row, 12] is null ? string.Empty : worksheet.Cells[row, 12].Value.ToString(),
                    Bairro = worksheet.Cells[row, 13].Value is null ? string.Empty : worksheet.Cells[row, 13].Value.ToString(),

                };
                response.Add(cliente);
            }
        }

        return response.ToList();
    }

    public static List<Cliente> ReadXlsAcoes(string NomeArquivo, string DirArqClienteOrigem, string DirArqClienteDestino)
    {
        var response = new List<Cliente>();
        var arquivo = DirArqClienteOrigem + NomeArquivo;

        FileInfo existingFile = new FileInfo(fileName: arquivo);

        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        using (ExcelPackage package = new ExcelPackage(existingFile))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int colCount = worksheet.Dimension.End.Column;
            int rowCount = worksheet.Dimension.End.Row;

            for (int row = 2; row < rowCount; row++)
            {
                var cliente = new Cliente
                {
                    IdHtml = int.Parse(worksheet.Cells[row, 1].Value.ToString()),
                    CodInterno = worksheet.Cells[row, 2].Value.ToString(),
                    CnpjParametro = worksheet.Cells[row, 4].Value.ToString(),
                    CnpjConsultado = worksheet.Cells[row, 5].Value.ToString(),
                    CnpjNumInscricao = worksheet.Cells[row, 6].Value.ToString(),
                    NomeEmpresarial = worksheet.Cells[row, 7].Value.ToString(),
                    NomeFantasia = worksheet.Cells[row, 8].Value.ToString(),

                    Logradouro = worksheet.Cells[row, 9].Value is null ? string.Empty : worksheet.Cells[row, 9].Value.ToString(),
                    Numero = worksheet.Cells[row, 10].Value is null ? string.Empty : worksheet.Cells[row, 10].Value.ToString(),
                    Complemento = worksheet.Cells[row, 11].Value is null ? string.Empty : worksheet.Cells[row, 11].Value.ToString(),
                    CEP = worksheet.Cells[row, 12] is null ? string.Empty : worksheet.Cells[row, 12].Value.ToString(),
                    Bairro = worksheet.Cells[row, 13].Value is null ? string.Empty : worksheet.Cells[row, 13].Value.ToString(),

                };
                response.Add(cliente);
            }
        }

        return response.ToList();
    }
}
