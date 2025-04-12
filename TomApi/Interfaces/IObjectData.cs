using TomApi.Models;

namespace TomApi.Interfaces;

public interface IObjectData : IDatabaseObject<Object_2D>
{
    public IEnumerable<Object_2D> Parent(string id);

    public bool RemoveByRoom(string roomId);
}