using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TomApi.Data;
using TomApi.Interfaces;
using TomApi.Models;

namespace TomApi.Controllers;

[Controller]
[Route("[controller]/")]
public class RoomController : Controller
{
    private ILogger<RoomController> _logger;
    private IRoomData _roomData;
    
    public RoomController(IRoomData roomData, ILogger<RoomController> logger)
    {
        _logger = logger;
        _roomData = roomData;
    }
    
    [HttpGet("all")]
    public IActionResult ReadAll()
    {
        try
        {
            return Ok(_roomData.ReadAll());
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message + "\n" + e.InnerException);
            return BadRequest("oopsie");
        }
    }

    [HttpGet("{id}")]
    public IActionResult Read(string id)
    {
        try
        {
            if(!Guid.TryParse(id, out _)) throw new($"Id not valid guid: {id}");
            return Ok(_roomData.Read(id));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message + "\n" + e.InnerException);
            return BadRequest("oopsie");
        }
    }

    [HttpPost]
    public IActionResult Write([FromBody] Room_2D room)
    {
        try
        {
            //Validate guid
            if(!Guid.TryParse(room.Id, out _)) throw new("Invalid guid supplied");
            
            //Check if roomname already exists
            
            return Ok(_roomData.Write(room));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message + "\n" + e.InnerException);
            return BadRequest("oopsie");
        }
    }   
    
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        try
        {
            return Ok(_roomData.Delete(id));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message + "\n" + e.InnerException);
            return BadRequest("oopsie");
        }
    }
}