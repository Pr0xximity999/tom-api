using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TomApi.Controllers;
using TomApi.Data;
using TomApi.Interfaces;
using TomApi.Models;

namespace TomApi.Tests;

[TestClass]
public class RoomDataTests
{
    [TestMethod]
    public void Create_WriteRoom_Success()
    {
        //Arrange
        var room = GenerateRoom(lenght: 50, height: 60, position:3);
        var roomController = GenerateRoomController(out _, out _, inputRoom:room, outputBoolean:true);
        
        //Act
        var response = roomController.Write(room);
       
        //Assert
        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
    }
    
    [TestMethod]
    public void Read_ReadRoomAtId_NotFound()
    {
        //Arrange
        var roomController = GenerateRoomController(out var id, out _, outputRoom:null);
       
       //Act
       var response = roomController.Read(id);    
       
       //Assert
       Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
    }
    
    /// <summary>
    /// Generates a room with random properties
    /// </summary>
    /// <returns></returns>
    private Room_2D GenerateRoom(string? id=null, string? userId = null, int? height=null, int? lenght = null, string? name = null, int? position=null)
    {
        Random random = new();
        return new Room_2D
        {
            Id = id ?? Guid.NewGuid().ToString(),
            User_Id = userId ?? Guid.NewGuid().ToString(),
            MaxHeight = height ?? random.Next(1, 255),
            MaxLength = lenght ?? random.Next(1, 255),
            Name = name ?? $"Room #{random.Next(1, 2555)}",
            position = position ?? random.Next(-255,255),
        };
    }
    
        /// <summary>
    /// Generates a post setup room controller,
    /// use GenerateEmptyRoomController If you want to tweak the constructor data.
    /// Use the output room if you want to be able to get true back from a mocked method using the room as parameter
    /// </summary>
    /// <param name="outId">Use this guid if you dont want to generate one for the controller method</param>
    /// <param name="inputId">The id being used in the data call</param>
    /// <param name="outRoom">The room being used in the data call, same as inputRoom, generates a new one if that one is left null</param>
    /// <param name="inputRoom">The room being used in the data call, same as outRoom</param>
    /// <param name="outputRoom">The room being returned from the data call.
    /// Used by Read and thus the exists checks. Will succeed if proceedure is not null and the id is the same as inputId</param>
    /// <param name="outputRooms">The rooms list being returned from the data call. Used by ReadAll</param>
    /// <param name="outputBoolean">The boolean being returned from the data call. Used by Write, Update, Delete, AddItem and RemoveItem</param>
    /// <returns></returns>
    private RoomController GenerateRoomController(out string outId, out Room_2D outRoom,
        string? inputId = null, Room_2D? inputRoom = null, string? inputName = null,
        Room_2D? outputRoom = null, List<Room_2D>? outputRooms = null, bool outputBoolean = false, string? outputUserId=null)
    {
        //set up input output data
        Mock<IRoomData> roomData = new();
        Mock<IAuthenticationService> auth = new();
        inputId ??= Guid.NewGuid().ToString();
        outId = inputId; //Lambdas do not accept out variables
        inputRoom ??= GenerateRoom(inputId);
        outRoom = inputRoom;
        inputName ??= "name #" + Random.Shared.Next();
        outputUserId ??= Guid.NewGuid().ToString();
        
        //Setup mock data layer
        auth.Setup(x => x.GetCurrentUserId()).Returns(outputUserId);
        
        roomData.Setup(x => x.ReadByUserId(outputUserId)).Returns(outputRooms!);
        roomData.Setup(x => x.ReadByName(inputName)).Returns(outputRoom);
        roomData.Setup(x => x.Read(inputId)).Returns(outputRoom);
        roomData.Setup(x => x.Write(inputRoom)).Returns(outputBoolean);
        roomData.Setup(x => x.Delete(inputId)).Returns(outputBoolean);
        
        //Create controller
        return GenerateEmptyRoomController(roomData: roomData);
    }    
    
    
    /// <summary>
    /// Generates an empty room controller, override the default parameters for customization
    /// </summary>
    /// <param name="roomData"></param>
    /// <param name="objectData"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    private RoomController GenerateEmptyRoomController(
        Mock<IRoomData>? roomData = null,
        Mock<IObjectData>? objectData = null,
        Mock<ILogger<RoomController>>? logger = null,
        Mock<IAuthenticationService>? auth = null)
    {
        roomData ??= new();
        objectData ??= new();
        logger ??= new();
        auth ??= new();

        return new RoomController(roomData.Object, objectData.Object, logger.Object, auth.Object);
    }
}