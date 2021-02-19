using AutoMapper;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Decorators.Exceptions;
using Fixit.Notification.Management.Lib.Mappers;
using Fixit.Notification.Management.Lib.Seeders;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fixit.Notification.Management.Lib.UnitTests
{
	[TestClass]
	public class TestBase
	{
		public Seeders.IFakeSeederFactory _fakeDtoSeederFactory;

		// Mocks
		protected Mock<IConfiguration> _fakeConfiguration;
		protected Mock<IDatabaseMediator> _fakeDatabaseMediator;
		protected Mock<IDatabaseTableMediator> _fakeDatabaseTableMediator;
		protected Mock<IDatabaseTableEntityMediator> _fakeDatabaseTableEntityMediator;
		protected Mock<INotificationHubClient> _fakeNotificationHubClient;
		protected Mock<IExceptionDecorator<OperationStatus>> _fakeOperationStatusExceptionDecorator;

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
