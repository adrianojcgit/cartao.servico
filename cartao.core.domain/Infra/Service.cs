using System.Text;
using System.Globalization;
using RabbitMQ.Client;
using Cartao.Domain.Domains.PropostaContext.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Cartao.Domain.Domains.PropostaContext.Contract;

namespace Cartao.Domain.Infra
{
    public sealed class Service : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly Proposta _proposta;
        private readonly ILogger<Service> _logger;
        private readonly IProposta _iproposta;

        string fileName = "";
        string sourcePath = "";
        string targetPath = "";
        string sourceFile = "";
        string destFile = "";
        decimal fatMensal;
        string porteEmpresaFinal = "";
        public Service(
            Proposta clientes,
            IConfiguration configuration,
            ILogger<Service> logger,
            IProposta proposta) => (_proposta, _logger, _configuration, _iproposta) = (clientes, logger, configuration, proposta);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Importando dados da planilha Excel: {time}", DateTimeOffset.UtcNow);

                    Thread t2 = new(new ThreadStart(AdicionarFilaRabbitMQ));
                    t2.Start();

                    _logger.LogWarning("Em pausa programada de 1 minutos. : {time}", DateTimeOffset.UtcNow);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
                Environment.Exit(1);
            }
        }

        private void AdicionarFilaRabbitMQ()
        {

            _logger.LogInformation("Importando dados MongoDb: {time}", DateTimeOffset.Now);
            try
            {
                fileName = _configuration.GetSection("Arquivos").GetSection("Arquivo").Value;
                sourcePath = _configuration.GetSection("Arquivos").GetSection("DirArqClienteOrigem").Value;
                targetPath = _configuration.GetSection("Arquivos").GetSection("DirArqClienteDestino").Value;
                sourceFile = Path.Combine(sourcePath, fileName);
                destFile = Path.Combine(targetPath, fileName);

                if (Directory.Exists(sourcePath))
                {
                    if (File.Exists(sourceFile))
                    {
                        var proposta = Proposta.ReadXlsProspect(fileName, sourcePath);
                        SetarParametros(proposta);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetarParametros(PropostaBaseDto proposta)
        {
            try
            {
                string porte = "";
                decimal fatAnual = 0;
                bool ativo;

                foreach (var item in proposta.Propostas)
                {
                    PropostaBaseDto propostaDtoItem = new PropostaBaseDto(new List<PropostaDto>(){ });
                    item.NumeroProposta = GerarNumeroProposta();
                    porte = PorteEmpresa();
                    fatAnual = FatBrutoAnul(porte);
                    ativo = AtivoInativo();
                    item.PorteEmpresa = porte;
                    item.FatBrutoAnual = fatAnual;
                    item.Ativo = ativo;
                    Guid g = Guid.NewGuid();
                    item.CodGuid = g.ToString();
                    item.ClienteOrigem = "MongoDb";
                    item.Enderecos = item.Enderecos;
                    item.Faturamentos = FaturamentoMensal(porte, Convert.ToInt32(item.FatBrutoAnual.ToString().Replace(",", "").Replace(".", "")));
                    item.PorteEmpresa = porteEmpresaFinal;
                    propostaDtoItem.Propostas.Add(item);
                    GravarProposta(propostaDtoItem);
                    
                    PrepararCanalEnfileirar(propostaDtoItem);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GravarProposta(PropostaBaseDto item)
        {
           //_iproposta.CreateAsync(item);
            _iproposta.Adicionar(item);
        }

        private static void PrepararCanalEnfileirar(PropostaBaseDto item)
        {
            try
            {
                var factorySQL = new ConnectionFactory()
                {
                    HostName = "localhost",
                    VirtualHost = "/",
                    Port = AmqpTcpEndpoint.UseDefaultPort,
                    UserName = "guest",
                    Password = "guest"
                };
                var connection = factorySQL.CreateConnection();
                var queueName = "proposta";
                var channel1 = CreateChannel(connection);
                AdicionarItemNaFila(channel1, queueName, item);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public static IModel CreateChannel(IConnection connection)
        {
            var channel = connection.CreateModel();
            return channel;
        }

        public static void AdicionarItemNaFila(IModel channel, string queueName, PropostaBaseDto item)
        {
            Task.Run(() =>
            {
                channel.ConfirmSelect();
                channel.BasicAcks += Channel_BasicAcks_MongoDb;
                channel.BasicNacks += Channel_BasicNacks_MongoDb;
                channel.BasicReturn += Channel_BasicReturn_MongoDb;
                //fila dlq
                channel.ExchangeDeclare("DeadLetterExchange", ExchangeType.Fanout);      // 1o. Declara o Exchange
                channel.QueueDeclare("dlq_proposta", true, false, false, null);
                channel.QueueBind("dlq_proposta", "DeadLetterExchange", "");
                var arguments = new Dictionary<string, object>()
                {
                    {"x-dead-letter-exchange", "DeadLetterExchange" }
                };
                //fim fila dlq
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: arguments);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                string message = JsonConvert.SerializeObject(item);
                Console.WriteLine("Mensagem: " + message);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("*********************************************************************************************************************");

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: properties,
                                     body: body,
                                     mandatory: true);

            });
        }

        private static void Channel_BasicNacks_MongoDb(object sender, RabbitMQ.Client.Events.BasicNackEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now:o} -> Basic Nack MongoDb");
        }

        private static void Channel_BasicAcks_MongoDb(object sender, RabbitMQ.Client.Events.BasicAckEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now:o} -> Basic Ack MongoDb");
        }

        private static void Channel_BasicReturn_MongoDb(object sender, RabbitMQ.Client.Events.BasicReturnEventArgs e)
        {
            //var message = Encoding.UTF8.GetString(e.Body.ToArray());
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine($"{DateTime.Now:o} -> Basic Return -> {message} ");
        }


        #region Métodos
        private void RemoverArquivoDiretorio(string fileRemove)
        {
            try
            {
                Directory.CreateDirectory(targetPath);
                File.Copy(sourceFile, destFile, true);

                if (Directory.Exists(sourcePath))
                {
                    string[] files = Directory.GetFiles(sourcePath);

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in files)
                    {
                        if (fileRemove == s)
                        {
                            // Use static Path methods to extract only the file name from the path.
                            fileName = Path.GetFileName(s);
                            destFile = Path.Combine(targetPath, fileName);
                            File.Move(s, destFile, true);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static string  GerarNumeroProposta()
        {
            Random randNum = new Random();
            var result = ((long)randNum.Next(100000000, 999999999)).ToString();
            return result;
        }

        private bool AtivoInativo()
        {
            int n = 1;
            int MAX = 2;
            char[] alphabet = { '0', '1' };
            Random random = new Random();
            string res = "";
            for (int i = 0; i < n; i++)
                res = res + alphabet[random.Next(0, MAX)];
            return res == "0" ? false : true;
        }

        private string PorteEmpresa()
        {
            int n = 1;
            int MAX = 3;
            char[] alphabet = { 'G', 'M', 'P' };
            Random random = new Random();
            string res = "";
            for (int i = 0; i < n; i++)
                res = res + alphabet[random.Next(0, MAX)];

            return res;
        }

        private decimal FatBrutoAnul(string tpPorte)
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            Random randNum = new Random();
            string result;
            decimal vlTotal;

            if (tpPorte == "P")
                result = ((long)randNum.Next(12000000, 99999999)).ToString();
            else if (tpPorte == "M")
                result = ((long)randNum.Next(100000000, 499999999)).ToString();
            else
                result = ((long)randNum.Next(500000000, 2147483647)).ToString();

            string v4 = result.Substring(result.Length - 2);
            string v5 = result.Substring(0, result.Length - 2);
            string v6 = v5 + "," + v4;
            vlTotal = Convert.ToDecimal(v6, culture);
            return vlTotal;
        }

        private List<FaturamentoDto> FaturamentoMensal(string tpPorte, int vlAnual)
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            Random randNum = new Random();
            List<FaturamentoDto> list = new List<FaturamentoDto>();
            string[] _valor = new string[12];
            int[] _arrMes = new int[12] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            int row = 1;
            int _ValorTotalAno = 0;
            try
            {
                if (tpPorte.ToUpper() == "P")
                {
                    for (int a = 0; a < 5; a++)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            FaturamentoDto fat = new FaturamentoDto();
                            _valor[i] = ((long)randNum.Next(10000000, vlAnual)).ToString();
                            fat.Valor = Convert.ToDecimal(decimal.Parse(_valor[i]).ToString("N2")); //_valor[i].Substring(0, _valor[i].Count() - 2) + "." + _valor[i].Substring(_valor[i].Count() - 2);
                            fat.Mes = DateTime.Now.AddMonths(-row).Month;
                            fat.Ano = DateTime.Now.AddMonths(-row).Year;
                            _ValorTotalAno = _ValorTotalAno + fat.Mes;
                            row++;
                            list.Add(fat);
                        }
                    }

                    porteEmpresaFinal = PorteValor(_ValorTotalAno, vlAnual);

                }
                else if (tpPorte.ToUpper() == "M")
                {
                    for (int a = 0; a < 5; a++)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            FaturamentoDto fat = new FaturamentoDto();
                            _valor[i] = ((long)randNum.Next(100000000, vlAnual)).ToString();
                            fat.Valor = Convert.ToDecimal(decimal.Parse(_valor[i]).ToString("N2"));
                            fat.Mes = DateTime.Now.AddMonths(-row).Month;
                            fat.Ano = DateTime.Now.AddMonths(-row).Year;
                            _ValorTotalAno = _ValorTotalAno + fat.Mes;
                            row++;
                            list.Add(fat);
                        }
                    }
                    porteEmpresaFinal = PorteValor(_ValorTotalAno, vlAnual);
                }
                else
                {
                    for (int a = 0; a < 5; a++)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            FaturamentoDto fat = new FaturamentoDto();
                            _valor[i] = ((long)randNum.Next(500000000, vlAnual)).ToString();
                            fat.Valor = Convert.ToDecimal(decimal.Parse(_valor[i]).ToString("N2"));
                            fat.Mes = DateTime.Now.AddMonths(-row).Month;
                            fat.Ano = DateTime.Now.AddMonths(-row).Year;
                            _ValorTotalAno = _ValorTotalAno + fat.Mes;
                            row++;
                            list.Add(fat);
                        }
                    }
                    porteEmpresaFinal = PorteValor(_ValorTotalAno, vlAnual);
                }
                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static string PorteValor(int _ValorTotalAno, int vlAnual)
        {
            string _porte = "";
            try
            {
                if (_ValorTotalAno <= vlAnual)
                {
                    _porte = "P";
                }
                else if (_ValorTotalAno > vlAnual && _ValorTotalAno <= vlAnual)
                {
                    _porte = "M";
                }
                else
                {
                    _porte = "G";
                }
                return _porte;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

    }
}