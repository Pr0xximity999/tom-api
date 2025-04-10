using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TomApi.Data;
using TomApi.Interfaces;
using TomApi.Models;

namespace TomApi.Controllers;

[Controller]
[Authorize]
[Route("[controller]/")]
public class RoomController : Controller
{
    private ILogger<RoomController> _logger;
    private IRoomData _roomData;
    private IAuthenticationService _authService;
    
    public RoomController(IRoomData roomData, ILogger<RoomController> logger, IAuthenticationService authService)
    {
        _logger = logger;
        _authService = authService;
        _roomData = roomData;
    }

    [HttpGet("all")]
    public IActionResult ReadById()
    {
        try
        {
            //Get all the rooms available to this user
            var rooms = _roomData.ReadByUserId(_authService.GetCurrentUserId());

            if (rooms == null) throw new("Reading rooms by user id resulted in null list");

            foreach (var room in rooms)
            {
                //Change undescores to spaces
                var newName = "";
                foreach (var character in room.Name)
                {
                    if (character == '_')
                    {
                        newName += " ";
                        continue;
                    }

                    newName += character;
                }
                room.Name = newName;
            }
                
            return Ok(rooms);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message + "\n" + e.InnerException);
            return BadRequest("oopsie");
        }
    }    
    
    [HttpGet("name/{name}")]
    public IActionResult ReadByName(string name)
    {
        try
        {
            //Get all the rooms available to this user
            var room = _roomData.ReadByName(name);

            if (room == null) throw new("Reading room by name resulted in null list");
            
            //Change undescores to spaces
            var newName = "";
            foreach (var character in room.Name)
            {
                if (character == '_')
                {
                    newName += " ";
                    continue;
                }

                newName += character;
            }
            room.Name = newName;
                
            return Ok(room);
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
            //Validate guid
            if(!Guid.TryParse(id, out _)) throw new($"Id not valid guid: {id}");
                
            //Get procedure by id
            var room = _roomData.Read(id);
                
            //Check if the id fetched anything
            if (room == null) throw new("Room id fetch resulted in null");
            
            //Change underscores to spaces
            var newName = "";
            foreach (var character in room.Name)
            {
                if (character == '_')
                {
                    newName += " ";
                    continue;
                }

                newName += character;
            }
            room.Name = newName;
                
            //Return result
            return Ok(room);
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
            //Assign id to room object
            room.Id = Guid.NewGuid().ToString();
            
            //Assign user id to the room object
            room.User_Id = _authService.GetCurrentUserId();

            if (room.position is < 0 or > 4) throw new("Room position must be an int between 0 and 5 (inclusive)");
            
            //Change spaces to underscores
            var newName = "";
            foreach (var character in room.Name)
            {
                if (character == ' ')
                {
                    newName += "_";
                    continue;
                }

                newName += character;
            }
            room.Name = newName;
            
            //Check if the room name already exists
            if (NameExists(room.Name)) throw new("Procedure with this id already exists");

            //Check if writing to table succeeded
            var result = _roomData.Write(room);
            if (!result) throw new("Writing procedure to table resulted in nothing happening");
                
            //Return result
            return Ok(result);
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
            //Validate guid
            if(!Guid.TryParse(id, out _))  throw new($"Id not valid guid: {id}");
                
            //Check if the procedure id already exists
            if (!IdExists(id)) throw new("Room with this id doesn't exist");
                
            //Check if deleting on table succeeded
            var result = _roomData.Delete(id);
            if (!result) throw new("Deleting procedure from table resulted in nothing happening");
                
            //Return result
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message + "\n" + e.InnerException);
            return BadRequest("oopsie");
        }
    }

    private bool NameExists(string name) => _roomData.ReadByName(name) != null;

    private bool IdExists(string id) => _roomData.Read(id) != null;
}