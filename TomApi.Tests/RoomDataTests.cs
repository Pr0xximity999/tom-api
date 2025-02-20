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
        Mock<ILogger<RoomController>> logger = new();
        Mock<IRoomData> roomData = new();
        
        //Simulate the datalayer succeeding
        roomData.Setup(x => x.Write(room)).Returns(success);
        
        var roomController = new RoomController(roomData.Object, logger.Object);
        
        //Act
        var response = roomController.Write(room);
       
        //Assert
        Assert.IsInstanceOfType(response, out OkObjectResult _);
    }
    
    [TestMethod]
    public void Read_ReadRoomAtId_NotFound()
    {
        
        //Arrange
        string roomId = Guid.NewGuid().ToString();
        Mock<ILogger<RoomController>> logger = new();
        Mock<IRoomData> roomData = new();
        
        //Simulate the datalayer failing because no object was found
        roomData.Setup(x => x.Read(roomId)).Throws(new Exception("No object with such id"));
        
       var roomController = new RoomController(roomData.Object, logger.Object);
       
       //Act
       var response = roomController.Read(roomId);
       
       //Assert
       Assert.IsInstanceOfType(response, out BadRequestObjectResult _);
    }

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