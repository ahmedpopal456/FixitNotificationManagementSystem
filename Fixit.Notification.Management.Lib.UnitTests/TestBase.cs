using AutoMapper;
using Fixit.Notification.Management.Lib.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fixit.Notification.Management.Lib.UnitTests
{
	[TestClass]
	public class TestBase
	{
		public TestBase()
		{
		}

		// Mapper
		protected MapperConfiguration _mapperConfiguration = new MapperConfiguration(config =>
		{
			config.AddProfile(new FixitNotificationMapper());
		});

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
