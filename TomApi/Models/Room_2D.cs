namespace TomApi.Models;

public class Room_2D {
    public string? Id { get; set; }
    public string User_Id { get; set; }
    public string Name { get; set; }
    public float MaxLength { get; set; }
    public float MaxHeight { get; set; }
    public int position { get; set; }
    
    public List<Object_2D> objects { get; set; }
}