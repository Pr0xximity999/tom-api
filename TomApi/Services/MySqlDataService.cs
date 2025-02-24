using Dapper;
using MySql.Data.MySqlClient;
using TomApi.Interfaces;

namespace TomApi.Services;
    /// <summary>
    /// Executes the given query and returns wether one or more row changed
    /// </summary>
    /// <param name="query">The ran query</param>
    /// <param name="parameters">The added query params</param>
    /// <returns></returns>
public class MySqlDataService : IDataService
{
    private IConfiguration _config;
    private const string CONN = "maria";
    public MySqlDataService(IConfiguration config)
    {
        _config = config;
    }

    public IEnumerable<T> QuerySql<T>(string query, object? parameters=null)
    {
        using var connection = new MySqlConnection(_config.GetConnectionString(CONN) ?? "");
        IEnumerable<T> result = connection.Query<T>(query, parameters);

        return result;
    }
    
    
    public T QueryFirstSql<T>(string query, object? parameters = null) => QuerySql<T>(query, parameters).First();
    
    
    public bool ExecuteSql(string query, object? parameters=null)
    {
        using var connection = new MySqlConnection(_config.GetConnectionString(CONN) ?? "");
        bool result = connection.Execute(query, parameters) > 0;

        return result;
    }
}