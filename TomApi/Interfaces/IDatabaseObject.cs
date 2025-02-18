using TomApi.Models;

namespace TomApi.Interfaces;

public interface IDatabaseObject<T>
{
    public IEnumerable<T> ReadAll();

    public Room_2D Read(string id);

    public bool Write(T room);

    public bool Delete(string id);


}