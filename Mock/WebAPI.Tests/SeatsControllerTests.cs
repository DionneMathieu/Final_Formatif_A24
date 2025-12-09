using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;
using WebAPI.Exceptions;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Tests;

[TestClass]
public class SeatsControllerTests
{
    Mock<SeatsService> serviceMock;
    Mock<SeatsController> controller;

    public SeatsControllerTests()
    {
        serviceMock = new Mock<SeatsService>();
        controller = new Mock<SeatsController>(serviceMock.Object) {CallBase = true};

        controller.Setup(c => c.UserId).Returns("11111");
    }

    [TestMethod]
    public void ReserveSeat()
    {
        Seat seat = new Seat();
        seat.Id = 1;
        seat.Number = 1;

        serviceMock.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Returns(seat);

        //trigger test Reserve seat
        var actionresult = controller.Object.ReserveSeat(seat.Number);

        //Confirme que le test retourne
        var result = actionresult.Result as OkObjectResult;
        Assert.IsNotNull(result);
    }
    [TestMethod]
    public void ReserveSeat_SeatAlreadyTaken()
    {
        serviceMock.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new SeatAlreadyTakenException());
        //trigger le reserve seat
        var actionresult = controller.Object.ReserveSeat(1);
        //confirme que le test retourne une erreur comme quoi le seat is already taken
        var result = actionresult.Result as UnauthorizedResult;
        Assert.IsNotNull(result);
    }
    [TestMethod]
    public void ReserveSeat_SeatOutOfBounds()
    {
        serviceMock.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new SeatOutOfBoundsException());

        var seatNumber = 1;
        //trigger le reserveseat
        var actionresult = controller.Object.ReserveSeat(seatNumber);
        //confirme que le test retourne une erreur comme quoi le seat est en dehors de la grid
        var result = actionresult.Result as NotFoundObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual("Could not find " + seatNumber, result.Value);

    }
    [TestMethod]
    public void ReserveSeat_UserAlreadySeated()
    {
        serviceMock.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new UserAlreadySeatedException());
        //trigger reserveseat
        var actionresult = controller.Object.ReserveSeat(1);
        //utilisateur a déjà une place réservé
        var result = actionresult.Result as BadRequestResult;
        Assert.IsNotNull(result);
    }



}
