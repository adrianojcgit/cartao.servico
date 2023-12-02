namespace Cartao.Domain.Infra
{
    public class DataBaseConfig : IDataBaseConfig
    {
        public string DataBaseName { get ; set; }
        public string ConnectionString { get; set; }
    }
}
