using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Empower.BillingManagement.Lib.UnitTests
{
  public class TestBase
  {
    public TestBase()
    {
    }

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
    }

    [AssemblyCleanup]
    public static void AfterSuiteTests()
    {
    }
  }
}