using TomApi.Interfaces;
using TomApi.Models;

namespace TomApi.Data;

public class RoomData : IRoomData
{
    private const string Table = "Room_2D";
    private readonly IDataService _dataService;
    public RoomData(IDataService dataService)
    {
        _dataService = dataService;
    }
    public IEnumerable<Room_2D> ReadAll()
    {
        string query = @$"SELECT * FROM {Table}";
        List<Room_2D> result = _dataService.QuerySql<Room_2D>(query).ToList();
        return result;
    }

    public Room_2D Read(string id)
    {
        string query = 
@$"SELECT * FROM {Table}
WHERE `id` = @Id";
        
        //Throw exception if no item found
        Room_2D result = _dataService.QuerySql<Room_2D>(query, new {Id = id}).FirstOrDefault() ?? throw new("No object with such id");
        return result;
    }

    public bool Write(Room_2D object2D)
    {
        string query =
$@"INSERT  INTO {Table}
(`Id`, `User_Id`, `Name`, `MaxLength`, `MaxHeight`)
VALUES(@Id, @User_Id, @Name, @MaxLength, @MaxHeight)";

        bool result = _dataService.ExecuteSql(query, new
        {
            object2D.Id,
            object2D.User_Id,
            object2D.Name,
            object2D.MaxLength,
            object2D.MaxHeight
        });
        
        if (!result) throw new("Writing object to table resulted in nothing happening");

        return result;
    }

    public bool Delete(string id)
    {
        string query =
$@"DELETE FROM {Table}
where `Id` = @Id";

        bool result = _dataService.ExecuteSql(query, new { Id = id });
        
        if (!result) throw new("Deleting object from table resulted in nothing happening");

        return result;
    }
    
    
}