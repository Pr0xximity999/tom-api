using TomApi.Models;

namespace TomApi.Interfaces;

public interface IRoomData : IDatabaseObject<Room_2D>
{
    public bool Update(Room_2D room);
    public IEnumerable<Room_2D> ReadByUserId(string id);
    public Room_2D? ReadByName(string name);
}