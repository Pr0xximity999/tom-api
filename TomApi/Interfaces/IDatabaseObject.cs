using TomApi.Models;

namespace TomApi.Interfaces;

public interface IDatabaseObject<T>
{
    public IEnumerable<T> ReadAll();

    public T? Read(string id);

    public bool Write(T room2D);
    
    public bool Delete(string id);


}