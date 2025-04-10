using TomApi.Models;

namespace TomApi.Interfaces;

public interface IRoomData : IDatabaseObject<Room_2D>
{
    public IEnumerable<Room_2D> ReadByUserId(string id);
    public Room_2D? ReadByName(string name);
}