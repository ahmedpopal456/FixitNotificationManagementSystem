using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NotificationWebApi.UnitTest
{

  [TestClass]
  public class ProgramTests
  {
    private Program _program;

    [TestInitialize]
    public void TestInitialize()
    {
      _program = new Program(); 
    }

    [TestMethod]
    public void DummyFunction_ReturnsTrue()
    {
      // Arrange

      // Act
      var result = _program.DummyFunction();

      // Assert
      Assert.IsTrue(result);
    }

    [TestCleanup]
    public void TestCleanup()
    {
      _program = null;
    }
  }
}
