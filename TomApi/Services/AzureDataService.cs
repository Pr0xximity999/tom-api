using Dapper;
using Microsoft.Data.SqlClient;
using TomApi.Interfaces;

namespace TomApi.Services;

public class AzureDataService : IDataService
{
    private IConfiguration _config;
    private const string CONN = "azure";
    public AzureDataService(IConfiguration config)
    {
        _config = config;
    }
    
    public IEnumerable<T> QuerySql<T>(string query, object? parameters=null)
    {
        using var connection = new SqlConnection(_config.GetConnectionString(CONN) ?? "");
        IEnumerable<T> result = connection.Query<T>(query, parameters);

        return result;
    }
    
    public T QueryFirstSql<T>(string query, object? parameters = null) => QuerySql<T>(query, parameters).First();
    
    public bool ExecuteSql(string query, object? parameters=null)
    {
        using var connection = new SqlConnection(_config.GetConnectionString(CONN) ?? "");
        bool result = connection.Execute(query, parameters) > 0;

        return result;
    }
}