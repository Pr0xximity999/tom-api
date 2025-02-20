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
            return Ok(_objectData.ReadAll());
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
            return Ok(_objectData.Read(id));
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
            //Validate guid
            if(!Guid.TryParse(object2D.Id, out _)) throw new("Invalid guid supplied");
            
            //Check if roomname already exists
            
            return Ok(_objectData.Write(object2D));
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
            return Ok(_objectData.Delete(id));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message + "\n" + e.InnerException);
            return BadRequest("oopsie");
        }
    }
}