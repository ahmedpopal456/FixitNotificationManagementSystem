using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications;
using Fixit.Notification.Management.Lib.Models.Notifications.Installations;
using Fixit.Notification.Management.Triggers.Functions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Triggers.UnitTests.Functions
{
	[TestClass]
	public class OnQueueTriggersTests : TestBase
	{
		private ILoggerFactory _fakeLoggerFactory;

		[TestInitialize]
		public void TestInitialize()
		{
			_fakeConfiguration = new Mock<IConfiguration>();
			_fakeNotificationHubClient = new Mock<INotificationHubClient>();
			_fakeLoggerFactory = new Mock<ILoggerFactory>().Object;
		}

		#region OnQueueNotifyUsers
		[TestMethod]
		public async Task NotifyUsers_AllRequests_ReturnsCompletedTask()
		{
			// Local mocks
			var _fakeNotificationInstallationMediator = new Mock<INotificationInstallationMediator>();
			var _fakeNotificationHubClient = new Mock<INotificationHubClient>();

			// Arrange
			var cancellationToken = CancellationToken.None;
			var notificationOutcome = new NotificationOutcome();
			var userIdList = new List<Guid> { Guid.Parse("3441a80b-cf00-41f5-80f1-b069f1d3cda6") };
			var deviceInstallations = _fakeDtoSeederFactory.CreateSeederFactory(new DeviceInstallationDto()).Select(deviceInstallation => deviceInstallation);
			var notificationDto = _fakeDtoSeederFactory.CreateSeederFactory(new NotificationDto()).First();
			var notificationJson = JsonConvert.SerializeObject(notificationDto);
			var onQueueNotifyUsers = new OnQueueNotifyUsers(_fakeConfiguration.Object,
																											_fakeLoggerFactory,
																											_fakeNotificationInstallationMediator.Object,
																											_fakeNotificationHubClient.Object);

			_fakeNotificationInstallationMediator.Setup(notificationInstallationMediator => notificationInstallationMediator.GetInstallationsAsync(It.IsAny<CancellationToken>(), null, null, It.IsAny<IList<Guid>>()))
																					 .Returns(Task.FromResult(deviceInstallations));

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.SendWindowsNativeNotificationAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
																.Returns(Task.FromResult(notificationOutcome));

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.SendAppleNativeNotificationAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
																.Returns(Task.FromResult(notificationOutcome));

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.SendFcmNativeNotificationAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
																.Returns(Task.FromResult(notificationOutcome));

			// Act
			var actionResult = await onQueueNotifyUsers.NotifyUsers(notificationJson, cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsTrue(Convert.ToBoolean(actionResult));
		}
		#endregion
	}
}
