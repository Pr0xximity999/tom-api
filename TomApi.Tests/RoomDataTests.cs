using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TomApi.Controllers;
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
        Room_2D room = GenerateRoom2D();
        bool success = true;
        Mock<IRoomData> roomData = new();
        
        //Simulate the datalayer succeeding
        roomData.Setup(x => x.Write(room)).Returns(success);
        
        var roomController = GenerateController(roomData: roomData);
        
        //Act
        var response = roomController.Write(room);
       
        //Assert
        Assert.IsInstanceOfType(response, typeof(OkObjectResult));
    }
    
    [TestMethod]
    public void Read_ReadRoomAtId_NotFound()
    {
        //Arrange
        string roomId = Guid.NewGuid().ToString();

        Mock<IRoomData> roomData = new();
        
        //Simulate the datalayer failing because no object was found
        roomData.Setup(x => x.Read(roomId)).Throws(new Exception("No object with such id"));
        
       var roomController = GenerateController(roomData: roomData);
       
       //Act
       var response = roomController.Read(roomId);    
       
       //Assert
       Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// Creates a controller, override the default used parameters for customization
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="roomData"></param>
    /// <param name="authService"></param>
    /// <returns></returns>
    private RoomController GenerateController(
        Mock<ILogger<RoomController>>? logger = null,
        Mock<IRoomData>? roomData = null,
        Mock<IAuthenticationService>? authService = null
        )
    {
        //Checking if the default values are unchanged and filling them otherwise
        logger ??= new();
        roomData ??= new();
        authService ??= new();

        return new RoomController(roomData.Object, logger.Object, authService.Object);
    }
    
    /// <summary>
    /// Generates a room with random properties
    /// </summary>
    /// <returns></returns>
    private Room_2D GenerateRoom2D()
    {
        Random random = new();
        return new Room_2D
        {
            Id = Guid.NewGuid().ToString(),
            MaxHeight = random.Next(1, 255),
            MaxLength = random.Next(1, 255),
            Name = $"Room #{random.Next(1, 2555)}",
            User_Id = Guid.NewGuid().ToString()
        };
    }
}