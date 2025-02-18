using Dapper;
using TomApi.Interfaces;
using TomApi.Models;
using MySql.Data.MySqlClient;

namespace TomApi.Data;

public class RoomData : IDatabaseObject<Room_2D>
{
    private IConfiguration _config;
    private const string TABLE = "Room_2D";
    public RoomData(IConfiguration config)
    {
        _config = config;
    }
    public IEnumerable<Room_2D> ReadAll()
    {
        string query = @$"SELECT * FROM {TABLE}";
        List<Room_2D> result = QuerySql<Room_2D>(query).ToList();
        return result;
    }

    public Room_2D Read(string id)
    {
        string query = 
@$"SELECT * FROM {TABLE}
WHERE `id` = @Id";
        
        //Throw exception if no item found
        Room_2D result = QuerySql<Room_2D>(query, new {Id = id}).FirstOrDefault() ?? throw new("No object with such id");
        return result;
    }

    public bool Write(Room_2D room)
    {
        //Valiaate id guid and recreate it if its not
        if (!Guid.TryParse(room.Id, out _)) room.Id = Guid.NewGuid().ToString();
        
        string query =
$@"INSERT  INTO {TABLE}
(`Id`, `User_Id`, `Name`, `MaxLength`, `MaxHeight`)
VALUES(@Id, @User_Id, @Name, @MaxLength, @MaxHeight)";

        bool result = ExecuteSql(query, new
        {
            Id = room.Id,
            User_Id = room.User_Id,
            Name = room.Name,
            Maxlength = room.MaxLength,
            MaxHeight = room.MaxHeight
        });
        
        if (!result) throw new("Writing object to table resulted in nothing happening");

        return result;
    }

    public bool Delete(string id)
    {
        string query =
$@"DELETE FROM {TABLE}
where `Id` = @Id";

        bool result = ExecuteSql(query, new { Id = id });
        
        if (!result) throw new("Deleting object fromt table resulted in nothing happening");

        return result;
    }
    
    /// <summary>
    /// Executes the given query and returns the result in a list.
    /// Use .FirstOrDefault() in case you expect only a single object
    /// </summary>
    /// <param name="query">The ran query</param>
    /// <param name="parameters">The added query params</param>
    /// <typeparam name="T">The type inside the collection the response will be casted to</typeparam>
    /// <returns></returns>
    private IEnumerable<T> QuerySql<T>(string query, object? parameters=null)
    {
        using var connection = new MySqlConnection(_config.GetConnectionString("Default") ?? "");
        IEnumerable<T> result = connection.Query<T>(query, parameters);

        return result;
    }    
    private bool ExecuteSql(string query, object? parameters=null)
    {
        using var connection = new MySqlConnection(_config.GetConnectionString("Default") ?? "");
        bool result = connection.Execute(query, parameters) > 0;

        return result;
    }
}