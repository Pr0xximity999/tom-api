namespace TomApi.Interfaces;

public interface IDataService
{
    public IEnumerable<T> QuerySql<T>(string query, object? parameters = null);
    public bool ExecuteSql(string query, object? parameters = null);
}