using Dapper;
using TomApi.Interfaces;
using TomApi.Models;
using MySql.Data.MySqlClient;
using TomApi.Services;

namespace TomApi.Data;

public class RoomData : IRoomData
{
    private const string TABLE = "Room_2D";
    private IDataService _dataService;
    public RoomData(IDataService dataService, IConfiguration config)
    {
        _dataService = dataService;
    }
    public IEnumerable<Room_2D> ReadAll()
    {
        string query = @$"SELECT * FROM {TABLE}";
        List<Room_2D> result = _dataService.QuerySql<Room_2D>(query).ToList();
        return result;
    }

    public Room_2D Read(string id)
    {
        string query = 
@$"SELECT * FROM {TABLE}
WHERE `id` = @Id";
        
        //Throw exception if no item found
        Room_2D result = _dataService.QuerySql<Room_2D>(query, new {Id = id}).FirstOrDefault() ?? throw new("No object with such id");
        return result;
    }

    public bool Write(Room_2D room)
    {
        string query =
$@"INSERT  INTO {TABLE}
(`Id`, `User_Id`, `Name`, `MaxLength`, `MaxHeight`)
VALUES(@Id, @User_Id, @Name, @MaxLength, @MaxHeight)";

        bool result = _dataService.ExecuteSql(query, new
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

        bool result = _dataService.ExecuteSql(query, new { Id = id });
        
        if (!result) throw new("Deleting object fromt table resulted in nothing happening");

        return result;
    }
    
    
}