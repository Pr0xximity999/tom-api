using System.ComponentModel.DataAnnotations;
namespace TomApi.Models;

public class Object_2D
{
    [Key]
    public string? Id { get; set; }
    
    [Required]
    public string Room2D_Id { get; set; }
    
    [Required]
    public string Prefab_Id { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float RotationZ { get; set; }
}