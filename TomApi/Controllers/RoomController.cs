using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private IObjectData _objectData;
    private IAuthenticationService _authService;
    
    public RoomController(IRoomData roomData, IObjectData objectData, ILogger<RoomController> logger, IAuthenticationService authService)
    {
        _roomData = roomData;
        _objectData = objectData;
        _logger = logger;
        _authService = authService;
    }

    [HttpGet("all")]
    public IActionResult ReadById()
    {
        try
        {
            //Get all the rooms available to this user
            var rooms = _roomData.ReadByUserId(_authService.GetCurrentUserId()).ToList();

            if (rooms == null) throw new("Reading rooms by user id resulted in null list");

            //Cant change a foreach variable 
            foreach (var room in rooms)
            {
                //Change spaces to underscores
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
                room.objects = _objectData.Parent(room.Id!).ToList();
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
            
            //Change spaces to underscores
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
            room.objects = _objectData.Parent(room.Id!).ToList();

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
            
            //Change spaces to underscores
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
            room.objects = _objectData.Parent(room.Id!).ToList();
                
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
            
            //Check if the room name already exists
            if (NameExists(room.Name)) throw new("Procedure with this id already exists");

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
    
    [HttpPut]
    public IActionResult Update([FromBody] Room_2D room)
    {
        try
        {
            //Validate guid
            if(!Guid.TryParse(room.Id, out _))  throw new($"Id not valid guid: {room.Id}");
                
            //Check if the procedure id doesn't exist
            if (!IdExists(room.Id)) throw new("Room with this id doesn't exist");
                
            //Check if updating to table succeeded
            var result = _roomData.Update(room);
            if (!result) throw new("Updating procedure to table resulted in nothing happening");
                
            //Return result
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message + "\n" + e.InnerException);
            return BadRequest();
        }
    }  
    
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        try
        {
            //Validate guid
            if(!Guid.TryParse(id, out _))  throw new($"Id not valid guid: {id}");
                
            //Check if the procedure id already exists and is bound to user
            var room = _roomData.Read(id);
            if (room == null) throw new("Room with this id doesn't exist");
            if (room.User_Id != _authService.GetCurrentUserId()) throw new("Specified room not bound to user who sent the request");
                
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