using System;
using Moq;
using ToH.BLL;
using ToH.Data;
using ToH.PL;
using ToH.PL.Screens;
using Xunit;

namespace ToH.Tests.Screens;

public class LoginScreenTest
{
    private Mock<IPrinter> _printer;
    private LoginScreen _uut;
    private Mock<IUi> _ui;
    private DashboardScreen _dashboardScreen;
    private Mock<IScreenFactory> _factory;
    private Mock<ISessionController> _controller;

    public LoginScreenTest()
    {
        _printer = new Mock<IPrinter>();
        _dashboardScreen = new DashboardScreen(new Mock<IHeroesController>().Object, _printer.Object);
        _factory = new Mock<IScreenFactory>();
        _factory.Setup(factory => factory.CreateScreen(
                It.Is<Type>(t => t == typeof(DashboardScreen)),
                It.Is<Hero>(h => h == null))
            )
            .Returns(_dashboardScreen);
        _ui = new Mock<IUi>();
        _ui.Setup(ui => ui.ScreenFactory).Returns(_factory.Object);
        _controller = new Mock<ISessionController>();

        _uut = new LoginScreen(_controller.Object, _printer.Object);
    }

    [Fact]
    public void ShouldShowWriteUsername_WhenInitIsCalled()
    {
        // Act
        _uut.Init();
        
        // Assert
        _printer.Verify(p => p.Clear(), Times.Once);
        _printer.Verify(p => p.PrintLine(It.Is<string>(s => s == "Write username:")), Times.Once);
    }

    [Fact]
    public void ShouldChangeScreenToDashboard_WhenTextIsEntered()
    {
        
        // Act
        _uut.Text(_ui.Object, "TestUsername");
        
        // Assert
        _ui.VerifySet(ui => ui.Screen = It.Is<DashboardScreen>(screen => screen == _dashboardScreen));
    }

    [Fact]
    public void ShouldSetUsernameInSessionController_WhenTextIsClickedWithUsername()
    {
        // Setup
        var username = "TestUsername";
        
        // Act
        _uut.Text(_ui.Object, username);
        
        // Assert
        _controller.VerifySet(c => c.Username = It.Is<string>(s => s == username));
    }

    [Fact]
    public void ShouldNotChangeScreen_WhenTextIsEmpty()
    {
        // Act
        _uut.Text(_ui.Object, "");
        
        // Assert
        _ui.VerifySet(ui => ui.Screen = It.Is<LoginScreen>(screen => screen == _uut), Times.Never());
    }
}