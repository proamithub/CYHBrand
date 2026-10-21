namespace CYHBrandData.Dapper
{
    public class DapperContext : IDapperContext
    {
        public string ConnectionString { get; }

        public DapperContext(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
