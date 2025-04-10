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
    
    public IEnumerable<Room_2D> ReadByUserId(string id)
    {
        string query = @$"SELECT * FROM {Table}
WHERE `User_ID` = @id";
        List<Room_2D> result = _dataService.QuerySql<Room_2D>(query, new{id = id}).ToList();
        return result;
    }

    public Room_2D? ReadByName(string name)
    {
        string query = 
            @$"SELECT * FROM {Table}
WHERE `name` = @Name";
        
        //Throw exception if no item found
        Room_2D? result = _dataService.QuerySql<Room_2D>(query, new { Name = name }).FirstOrDefault();
        return result;
    }

    public Room_2D? Read(string id)
    {
        string query = 
@$"SELECT * FROM {Table}
WHERE `id` = @Id";
        
        //Throw exception if no item found
        Room_2D? result = _dataService.QuerySql<Room_2D>(query, new { Id = id }).FirstOrDefault();
        return result;
    }

    public bool Write(Room_2D object2D)
    {
        string query =
$@"INSERT  INTO {Table}
(`Id`, `User_Id`, `Name`, `MaxLength`, `MaxHeight`, `Position`)
VALUES(@Id, @User_Id, @Name, @MaxLength, @MaxHeight, @Position)";

        bool result = _dataService.ExecuteSql(query, object2D);

        return result;
    }

    public bool Update(Room_2D object2D)
    {
        throw new NotImplementedException();
    }

    public bool Delete(string id)
    {
        string query =
$@"DELETE FROM {Table}
where `Id` = @Id";

        bool result = _dataService.ExecuteSql(query, new { Id = id });

        return result;
    }
}