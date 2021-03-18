using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Notification.Management.Lib.Decorators;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications;
using Fixit.Notification.Management.Lib.Models.Notifications.Installations;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Triggers.Functions
{
	public class OnQueueNotifyUsers
	{
		private readonly INotificationInstallationMediator _notificationInstallationMediator;
		private readonly INotificationHubClient _notificationHubClient;
		private readonly ILogger _logger;

		public OnQueueNotifyUsers(IConfiguration configurationProvider,
															ILoggerFactory loggerFactory,
															INotificationInstallationMediator notificationInstallationMediator,
															INotificationHubClient notificationHubClient)
		{
			_logger = loggerFactory.CreateLogger<OnQueueNotifyUsers>();
			_notificationInstallationMediator = notificationInstallationMediator ?? throw new ArgumentNullException($"{nameof(OnQueueNotifyUsers)} expects a value for {nameof(notificationInstallationMediator)}... null argument was provided");
			_notificationHubClient = notificationHubClient ?? throw new ArgumentNullException($"{nameof(OnQueueNotifyUsers)} expects a value for {nameof(notificationHubClient)}... null argument was provided");
		}

		[FunctionName("OnQueueNotifyUsers")]
		public async Task RunAsync([QueueTrigger("fixit-dev-nms-queue", Connection = "FIXIT-NMS-STORAGEACCOUNT-CS")] string queuedNotificationMessage, CancellationToken cancellationToken)
		{
			await NotifyUsers(queuedNotificationMessage, cancellationToken);
		}

		public async Task<int> NotifyUsers(string queuedNotificationMessage, CancellationToken cancellationToken)
		{
			int taskComplete = 1;

			// validate queue message
			NotificationDto notificationMessage = JsonConvert.DeserializeObject<NotificationDto>(queuedNotificationMessage);
			if (notificationMessage == null)
			{
				throw new InvalidOperationException($"{nameof(OnQueueNotifyUsers)} was unable to properly deserialize {nameof(queuedNotificationMessage)}...");
			}

			// convert tags
			IList<string> tags = notificationMessage.Tags.Select(item => $"{item.Key}:{item.Value}").ToList();

			// get userIds (recipientIds)
			IEnumerable<Guid> recipientIds = notificationMessage.Recipients.Select(userSummaryDto => userSummaryDto.Id).ToList();
			tags.Add($"userId:{recipientIds.First()}");

			// get device installations
			var deviceInstallations = await _notificationInstallationMediator.GetInstallationsAsync(cancellationToken, userIds: recipientIds);

			NotificationOutcome notificationOutcome = null;
			Parallel.ForEach(deviceInstallations, async deviceIntallation =>
			{
				switch (deviceIntallation.Platform)
				{
					case NotificationPlatform.Wns:
						// Windows 8.1 / Windows Phone 8.1
						var toast = new WindowsNativePayloadDecorator(notificationMessage).GetBase64StringConversion();
						notificationOutcome = await _notificationHubClient.SendWindowsNativeNotificationAsync(toast, tags, cancellationToken);
						break;
					case NotificationPlatform.Apns:
						// iOS
						var alert = new ApplePayloadDecorator(notificationMessage).GetBase64StringConversion();
						notificationOutcome = await _notificationHubClient.SendAppleNativeNotificationAsync(alert, tags, cancellationToken);
						break;
					case NotificationPlatform.Fcm:
						// Android
						var notif = new FirebasePayloadDecorator(notificationMessage).GetBase64StringConversion();
						notificationOutcome = await _notificationHubClient.SendFcmNativeNotificationAsync(notif, tags, cancellationToken);
						break;
				}

				// check notification outcome
				if (notificationOutcome == null
						&& ((notificationOutcome.State == NotificationOutcomeState.Abandoned)
						|| (notificationOutcome.State == NotificationOutcomeState.Unknown)))
				{
					taskComplete = 0;
					var errorMessage = $"Failed to notify user id {deviceIntallation.UserId} with device id {deviceIntallation.InstallationId} and notification outcoume {notificationOutcome?.State}...";
					_logger.LogError(errorMessage);
					throw new ArgumentNullException(errorMessage);
				}
			});
			return taskComplete;
		}
	}
}
