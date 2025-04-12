using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TomApi.Interfaces;
using TomApi.Models;

namespace TomApi.Controllers;

[Controller]
[Authorize]
[Route("[controller]/")]
public class ObjectController : Controller
{
    private ILogger<ObjectController> _logger;
    private IObjectData _objectData;
    
    
    public ObjectController(IObjectData objectData, ILogger<ObjectController> logger)
    {
        _logger = logger;
        _objectData = objectData;
    }
    
    [HttpGet("all")]
    public IActionResult ReadAll()
    {
        try
        {
            var objects = _objectData.ReadAll();

            //check if any proper response was made
            if (objects == null) throw new("Reading objects resulted in null list");
            
            return Ok(objects);
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
            var object2D = _objectData.Read(id);

            //If not found
            if (object2D == null) throw new("Reading object by id resulted in none found");
                
            return Ok(object2D);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message + "\n" + e.InnerException);
            return BadRequest("oopsie");
        }
    }

    [HttpPost]
    public IActionResult Write([FromBody] Object_2D object2D)
    {
        try
        {
            //Assign id to 2d object
            object2D.Id = Guid.NewGuid().ToString();
            
            //Check if writing to table succeeded
            var result = _objectData.Write(object2D);
            
            if (!result) throw new("Writing object to table resulted in nothing happening");
                
            //Return result
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message + "\n" + e.InnerException);
            return BadRequest("oopsie");
        }
    }  
    
    [HttpPost("multiple/{roomId}")]
    public IActionResult WriteMultiple(string roomId, [FromBody] List<Object_2D> object2Ds)
    {
        try
        {
            //First wipe the existing objects
            _objectData.RemoveByRoom(roomId);
            
            if (object2Ds.Count == 0) return Ok(true);
            foreach (var object2D in object2Ds)
            {
                //Assign id to 2d object
                object2D.Id = Guid.NewGuid().ToString();

                //Check if writing to table succeeded
                var result = _objectData.Write(object2D);

                if (!result) throw new("Writing object to table resulted in nothing happening");
            }

            //Return result
            return Ok(true);
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
                
            //Check if the procedure id already exists and is bound to user
            if(!Exists(id)) throw new("Object with this id doesn't exist");
                
            //Check if deleting on table succeeded
            var result = _objectData.Delete(id);
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

    private bool Exists(string id) => _objectData.Read(id) != null;
}