namespace Cartao.Domain.Infra
{
    public interface IDataBaseConfig
    {
        string DataBaseName { get; set; }
        string ConnectionString { get; set; }
    }
}
