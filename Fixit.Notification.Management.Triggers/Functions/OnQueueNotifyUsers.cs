using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Notification.Management.Lib.Decorators;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications;
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
		private readonly string UserPrefix = "userId";

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
		public async Task RunAsync([QueueTrigger("%FIXIT-NMS-QUEUE-NAME%", Connection = "FIXIT-NMS-STORAGEACCOUNT-CS")] string queuedNotificationMessage, CancellationToken cancellationToken)
		{
			await NotifyUsers(queuedNotificationMessage, cancellationToken);
		}

		public async Task<int> NotifyUsers(string queuedNotificationMessage, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			int taskComplete = 1;

			// validate queue message
			NotificationDto notificationMessage = JsonConvert.DeserializeObject<NotificationDto>(queuedNotificationMessage);
			if (notificationMessage == null)
			{
				throw new InvalidOperationException($"{nameof(OnQueueNotifyUsers)} was unable to properly deserialize {nameof(queuedNotificationMessage)}...");
			}

			// convert tags from notification payload
			IEnumerable<string> notifPayloadTags = notificationMessage.Tags.Select(item => $"{item.Key}:{item.Value}").ToList();

			// get userIds (recipientIds)
			IEnumerable<Guid> recipientIds = notificationMessage.Recipients.Select(userSummaryDto => userSummaryDto.Id).ToList();

			// get device installations
			var deviceInstallations = await _notificationInstallationMediator.GetInstallationsAsync(cancellationToken, userIds: recipientIds);
			deviceInstallations = deviceInstallations.GroupBy(deviceInstallation => deviceInstallation.UserId).Select(groupedDI => groupedDI.First());

			NotificationOutcome notificationOutcome = null;
			Parallel.ForEach(deviceInstallations, async deviceIntallation =>
			{
				// combine all tags and keep distinct values
				var currentTags = new List<string>();
				currentTags.AddRange(notifPayloadTags);
				currentTags.AddRange(deviceIntallation.Tags.Select(tag => $"{tag.Key}:{tag.Value}"));
				currentTags.Add($"{UserPrefix}:{deviceIntallation.UserId}");
				currentTags = currentTags.Distinct().ToList();

				// _logger.LogInformation($"{nameof(OnQueueNotifyUsers)}: {deviceIntallation.InstallationId} has tags {currentTags}");

				// send through the appropriate service
				switch (deviceIntallation.Platform)
				{
					case NotificationPlatform.Wns:
						// Windows 8.1 / Windows Phone 8.1
						var toast = new WindowsNativePayloadDecorator(notificationMessage).GetBase64StringConversion();
						notificationOutcome = await _notificationHubClient.SendWindowsNativeNotificationAsync(toast, currentTags, cancellationToken);
						break;
					case NotificationPlatform.Apns:
						// iOS
						var alert = new ApplePayloadDecorator(notificationMessage).GetBase64StringConversion();
						notificationOutcome = await _notificationHubClient.SendAppleNativeNotificationAsync(alert, currentTags, cancellationToken);
						break;
					case NotificationPlatform.Fcm:
						// Android
						var notif = new FirebasePayloadDecorator(notificationMessage).GetBase64StringConversion();
						notificationOutcome = await _notificationHubClient.SendFcmNativeNotificationAsync(notif, currentTags, cancellationToken);
						break;
				}

				// check notification outcome
				if (notificationOutcome == null
						&& ((notificationOutcome.State == NotificationOutcomeState.Abandoned)
						|| (notificationOutcome.State == NotificationOutcomeState.Unknown)))
				{
					taskComplete = 0;
					var errorMessage = $"{nameof(OnQueueNotifyUsers)} failed to notify user id {deviceIntallation.UserId} with device id {deviceIntallation.InstallationId} and notification outcome {notificationOutcome?.State}...";
					_logger.LogError(errorMessage);
					throw new ArgumentNullException(errorMessage);
				}
			});
			return taskComplete;
		}
	}
}
