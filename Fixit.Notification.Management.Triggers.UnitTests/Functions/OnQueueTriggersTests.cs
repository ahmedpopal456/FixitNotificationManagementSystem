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
		private OnQueueNotifyUsers _onQueueNotifyUsers;
		private ILoggerFactory _fakeLoggerFactory;
		private NotificationOutcome _notificationOutcome;
		private CancellationToken _cancellationToken;
		private IList<Guid> _userIdList;
		private IEnumerable<DeviceInstallationDto> _deviceInstallations;
		private string _notificationJson;

		private Mock<INotificationInstallationMediator> _fakeNotificationInstallationMediator;

		[TestInitialize]
		public void TestInitialize()
		{
			_fakeConfiguration = new Mock<IConfiguration>();
			_fakeNotificationHubClient = new Mock<INotificationHubClient>();
			_fakeLoggerFactory = new Mock<ILoggerFactory>().Object;
			_fakeNotificationInstallationMediator = new Mock<INotificationInstallationMediator>();

			_cancellationToken = CancellationToken.None;
			_notificationOutcome = new NotificationOutcome();
			_userIdList = new List<Guid> { Guid.Parse("3441a80b-cf00-41f5-80f1-b069f1d3cda6") };
			_deviceInstallations = _fakeDtoSeederFactory.CreateSeederFactory(new DeviceInstallationDto()).Select(deviceInstallation => deviceInstallation);

			var notificationDto = _fakeDtoSeederFactory.CreateSeederFactory(new NotificationDto()).First();
			_notificationJson = JsonConvert.SerializeObject(notificationDto);
			_onQueueNotifyUsers = new OnQueueNotifyUsers(_fakeConfiguration.Object,
																										_fakeLoggerFactory,
																										_fakeNotificationInstallationMediator.Object,
																										_fakeNotificationHubClient.Object);
			

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.SendWindowsNativeNotificationAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
																.ReturnsAsync(_notificationOutcome);

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.SendAppleNativeNotificationAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
																.ReturnsAsync(_notificationOutcome);

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.SendFcmNativeNotificationAsync(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
																.ReturnsAsync(_notificationOutcome);
		}

		#region OnQueueNotifyUsers
		[TestMethod]
		public async Task NotifyUsers_FcmDevice_ReturnsCompletedTask()
		{
			// Arrange
			_fakeNotificationInstallationMediator.Setup(notificationInstallationMediator => notificationInstallationMediator.GetInstallationsAsync(It.IsAny<CancellationToken>(), null, null, It.IsAny<IList<Guid>>()))
																					 .ReturnsAsync(_deviceInstallations);

			// Act
			var actionResult = await _onQueueNotifyUsers.NotifyUsers(_notificationJson, _cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsTrue(Convert.ToBoolean(actionResult));
		}

		[TestMethod]
		public async Task NotifyUsers_ApnsDevice_ReturnsCompletedTask()
		{
			// Arrange
			_deviceInstallations.Select(deviceInstallation => { deviceInstallation.Platform = NotificationPlatform.Apns; return deviceInstallation; }).ToList();
			_fakeNotificationInstallationMediator.Setup(notificationInstallationMediator => notificationInstallationMediator.GetInstallationsAsync(It.IsAny<CancellationToken>(), null, null, It.IsAny<IList<Guid>>()))
																					 .ReturnsAsync(_deviceInstallations);

			// Act
			var actionResult = await _onQueueNotifyUsers.NotifyUsers(_notificationJson, _cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsTrue(Convert.ToBoolean(actionResult));
		}

		[TestMethod]
		public async Task NotifyUsers_WnsDevice_ReturnsCompletedTask()
		{
			// Arrange
			_deviceInstallations.Select(deviceInstallation => { deviceInstallation.Platform = NotificationPlatform.Wns; return deviceInstallation; }).ToList();

			_fakeNotificationInstallationMediator.Setup(notificationInstallationMediator => notificationInstallationMediator.GetInstallationsAsync(It.IsAny<CancellationToken>(), null, null, It.IsAny<IList<Guid>>()))
																					 .ReturnsAsync(_deviceInstallations);

			// Act
			var actionResult = await _onQueueNotifyUsers.NotifyUsers(_notificationJson, _cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsTrue(Convert.ToBoolean(actionResult));
		}
		#endregion
	}
}
