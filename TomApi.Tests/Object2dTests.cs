using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TomApi.Controllers;
using TomApi.Data;
using TomApi.Interfaces;
using TomApi.Models;

namespace TomApi.Tests;

[TestClass]
public class Object2dTests
{
    
    //Writing tests
    [TestMethod]
    public void Create_WriteObject_Success()
    {
        //Arrange
        var objectController = GenerateObjectController(out _, out var object2D, outputBoolean:true);
        
        //Act
        var response = objectController.Write(object2D);
       
        //Assert
        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
    }
    
    [TestMethod]
    public void Create_WriteObject_Failed()
    {
        //Arrange
        var objectController = GenerateObjectController(out _, out var object2D, outputBoolean:false);
        
        //Act
        var response = objectController.Write(object2D);
       
        //Assert
        Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
    }

    
    //Reading tests
    [TestMethod]
    public void Read_ReadObjectAtId_Success()
    {
        //Arrange
        var objectController = GenerateObjectController(out var id, out _, outputObject:GenerateObject());
       
        //Act
        var response = objectController.Read(id);    
       
        //Assert
        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
    }
    
    [TestMethod]
    public void Read_ReadObjectAtId_NotFound()
    {
        //Arrange
        var objectController = GenerateObjectController(out var id, out _, outputObject:null);
       
       //Act
       var response = objectController.Read(id);    
       
       //Assert
       Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
    }
    
    [TestMethod]
    public void Read_ReadObjectAtId_IdNotValid()
    {
        //Arrange
        var objectController = GenerateObjectController(out var id, out _, inputId:"invalid");
       
        //Act
        var response = objectController.Read(id);    
       
        //Assert
        Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
    }
    
    /// <summary>
    /// Generates a object with random properties
    /// </summary>
    /// <returns></returns>
    private Object_2D GenerateObject(string? id=null, float? positionX = null, float? positionY = null, string? prefabId=null, string? room2dId = null)
    {
        Random random = new();
        return new Object_2D
        {
            Id = id ?? Guid.NewGuid().ToString(),
            PositionX = positionX ?? random.Next(0,255),
            PositionY = positionY ?? random.Next(0,255),
            RotationZ = 0,
            Prefab_Id = prefabId ?? Guid.NewGuid().ToString(),
            ScaleX = 1,
            ScaleY = 1,
            Room2D_Id = room2dId ?? Guid.NewGuid().ToString(),
        };
    }
    
    /// <summary>
    /// Generates a post setup object controller,
    /// use GenerateEmptyObjectController If you want to tweak the constructor data.
    /// Use the output object if you want to be able to get true back from a mocked method using the object as parameter
    /// </summary>
    /// <param name="outId">Use this guid if you dont want to generate one for the controller method</param>
    /// <param name="inputId">The id being used in the data call</param>
    /// <param name="outObject">The object being used in the data call, same as inputObject, generates a new one if that one is left null</param>
    /// <param name="inputObject">The object being used in the data call, same as outObject</param>
    /// <param name="outputObject">The object being returned from the data call.
    /// Used by Read and thus the exists checks. Will succeed if proceedure is not null and the id is the same as inputId</param>
    /// <param name="outputObjects">The objects list being returned from the data call. Used by ReadAll</param>
    /// <param name="outputBoolean">The boolean being returned from the data call. Used by Write, Update, Delete, AddItem and RemoveItem</param>
    /// <returns></returns>
    private ObjectController GenerateObjectController(out string outId, out Object_2D outObject,
        string? inputId = null, Object_2D? inputObject = null,
        Object_2D? outputObject = null, List<Object_2D>? outputObjects = null, bool outputBoolean = false)
    {
        //set up input output data
        Mock<IObjectData> objectData = new();
        Mock<IAuthenticationService> auth = new();
        inputId ??= Guid.NewGuid().ToString();
        outId = inputId; //Lambdas do not accept out variables
        inputObject ??= GenerateObject(inputId);
        outObject = inputObject;
        
        //Setup mock data layer
        objectData.Setup(x => x.Read(inputId)).Returns(outputObject);
        objectData.Setup(x => x.Write(inputObject)).Returns(outputBoolean);
        objectData.Setup(x => x.Delete(inputId)).Returns(outputBoolean);
        objectData.Setup(x => x.Parent(inputId)).Returns(outputObjects!);
        
        //Create controller
        return GenerateEmptyObjectController(objectData: objectData, auth:auth);
    }    
    
    
    /// <summary>
    /// Generates an empty object controller, override the default parameters for customization
    /// </summary>
    /// <param name="objectData"></param>
    /// <param name="objectData"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    private ObjectController GenerateEmptyObjectController(
        Mock<IObjectData>? objectData = null,
        Mock<ILogger<ObjectController>>? logger = null,
        Mock<IAuthenticationService>? auth = null)
    {
        objectData ??= new();
        objectData ??= new();
        logger ??= new();
        auth ??= new();

        return new ObjectController(objectData.Object, logger.Object);
    }
}