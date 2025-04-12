using TomApi.Interfaces;
using TomApi.Models;

namespace TomApi.Data;

public class ObjectData : IObjectData
{
    private const string Table = "Object_2D";
    private readonly IDataService _dataService;
    
    public ObjectData(IDataService dataService)
    {
        _dataService = dataService;
    }
    public IEnumerable<Object_2D> ReadAll()
    {
        string query = $"SELECT * FROM {Table}";
        IEnumerable<Object_2D> result = _dataService.QuerySql<Object_2D>(query);
        return result;
    }

    public Object_2D Read(string id)
    {
        string query = 
            @$"SELECT * FROM {Table}
WHERE `id` = @Id";
        
        //Throw exception if no item found
        Object_2D result = _dataService.QuerySql<Object_2D>(query, new {Id = id}).FirstOrDefault() ?? throw new("No object with such id");
        return result;
    }

    public bool Write(Object_2D room2D)
    {
        string query =
            $@"INSERT  INTO {Table}
(`Id`, `Room2D_Id`, `Prefab_Id`, `PositionX`, `PositionY`, `ScaleX`, `ScaleY`, `RotationZ`)
VALUES(@Id, @Room2D_Id, @Prefab_Id, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ)";

        bool result = _dataService.ExecuteSql(query, room2D);
        
        if (!result) throw new("Writing object to table resulted in nothing happening");

        return result;
    }

    public bool Delete(string id)
    {
        string query =
            $@"DELETE FROM {Table}
where `Id` = @Id";

        bool result = _dataService.ExecuteSql(query, new { Id = id });

        return result;
    }

    public IEnumerable<Object_2D> Parent(string id)
    {
        string query = $"SELECT * FROM {Table} WHERE `Room2D_Id` = @id";
        IEnumerable<Object_2D> result = _dataService.QuerySql<Object_2D>(query, new {id});
        return result;
    }

    public bool RemoveByRoom(string roomId)
    {
        string query =
            $@"DELETE FROM {Table}
where `Room2D_Id` = @roomId";

        bool result = _dataService.ExecuteSql(query, new { roomId });

        return result;
    }
}