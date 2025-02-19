using Dapper;
using MySql.Data.MySqlClient;
using TomApi.Interfaces;

namespace TomApi.Services;

public class MySqlDataService : IDataService
{
    private IConfiguration _config;
    private const string CONN = "maria";
    public MySqlDataService(IConfiguration config)
    {
        _config = config;
    }
    
    /// <summary>
    /// Executes the given query and returns the result in a list.
    /// Use .FirstOrDefault() in case  you expect only a single object
    /// </summary>
    /// <param name="query">The ran query</param>
    /// <param name="parameters">The added query params</param>
    /// <typeparam name="T">The type inside the collection the response will be casted to</typeparam>
    /// <returns></returns>
    public IEnumerable<T> QuerySql<T>(string query, object? parameters=null)
    {
        using var connection = new MySqlConnection(_config.GetConnectionString(CONN) ?? "");
        IEnumerable<T> result = connection.Query<T>(query, parameters);

        return result;
    }   
    
    /// <summary>
    /// Executes the given query and returns wether one or more row changed
    /// </summary>
    /// <param name="query">The ran query</param>
    /// <param name="parameters">The added query params</param>
    /// <returns></returns>
    public bool ExecuteSql(string query, object? parameters=null)
    {
        using var connection = new MySqlConnection(_config.GetConnectionString(CONN) ?? "");
        bool result = connection.Execute(query, parameters) > 0;

        return result;
    }
}