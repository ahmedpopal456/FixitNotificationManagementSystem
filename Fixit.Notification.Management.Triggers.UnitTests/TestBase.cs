using AutoMapper;
using Fixit.Core.Storage.Queue.Mediators;
using Fixit.Notification.Management.Lib.Mappers;
using Fixit.Notification.Management.Lib.Seeders;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fixit.Notification.Management.Triggers.UnitTests
{
	[TestClass]
	public class TestBase
	{
		public IFakeSeederFactory _fakeDtoSeederFactory;

		// Mock
		protected Mock<IConfiguration> _fakeConfiguration;
		protected Mock<INotificationHubClient> _fakeNotificationHubClient;
		protected Mock<IQueueServiceClientMediator> _fakeQueueServiceClientMediator;

		public TestBase()
		{
			_fakeDtoSeederFactory = new FakeDtoSeederFactory();
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
