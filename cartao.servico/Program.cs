using Cartao.Domain.Domains.PropostaContext.Contract;
using Cartao.Domain.Domains.PropostaContext.Repositories;
using Cartao.Domain.Domains.PropostaContext.Services;
using Cartao.Domain.Infra;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Options;
//using Cartao.Domain.Domains.PropostaContext.Repositories;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
         .UseWindowsService(options =>
         {
             options.ServiceName = "cartao.servico";
         })
         .ConfigureServices((context, services) =>
         {
             LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(services);
             services.AddSingleton<Proposta>();
             services.AddSingleton<IProposta, PropostaServices>();
             services.AddHostedService<Service>();
            //services.AddScoped<ICatalogContext, CatalogContext>();
             services.Configure<DataBaseConfig>(context.Configuration.GetSection(nameof(DataBaseConfig)));
             services.AddSingleton<IDataBaseConfig>(sp => sp.GetRequiredService<IOptions<DataBaseConfig>>().Value);
             services.AddSingleton<IPropostaRepository, PropostaRepository>();
         })
         .ConfigureLogging((context, logging) =>
         {
             logging.AddConfiguration(
                 context.Configuration.GetSection("Logging"));
         })
         //.ConfigureLogging((context1, databaseSettings) =>
         //{
         //    databaseSettings.AddConfiguration(
         //        context1.Configuration.GetSection("DatabaseSettings").GetSection("ConnectionString"));
         //})
         .Build();

        await host.RunAsync();
    }
}