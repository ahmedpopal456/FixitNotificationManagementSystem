using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Triggers
{
	public class OnQueueNotifyUsers
	{
    private readonly INotificationInstallationMediator _notificationInstallationMediator;
    private readonly NotificationHubClient _notificationHubClient;
		private readonly string _queueName;
		private readonly string _anhConnectionString;
		private readonly string _anhName;

    public OnQueueNotifyUsers(IConfiguration configurationProvider, INotificationInstallationMediator notificationInstallationMediator)
		{
			_queueName = configurationProvider["FIXIT-NMS-QUEUE-NAME"];
      _anhConnectionString = configurationProvider["FIXIT-NMS-ANH-CS"];
      _anhName = configurationProvider["FIXIT-NMS-ANH-NAME"];

			if (string.IsNullOrWhiteSpace(_queueName))
			{
				throw new ArgumentNullException($"{nameof(OnQueueNotifyUsers)} expects the {nameof(configurationProvider)} to have defined the Queue Name as {{FIXIT-NMS-QUEUE-NAME}} ");
			}

      if (string.IsNullOrWhiteSpace(_anhConnectionString))
      {
        throw new ArgumentNullException($"{nameof(OnQueueNotifyUsers)} expects the {nameof(configurationProvider)} to have defined the Queue Name as {{FIXIT-NMS-ANH-CS}} ");
      }

      if (string.IsNullOrWhiteSpace(_anhName))
      {
        throw new ArgumentNullException($"{nameof(OnQueueNotifyUsers)} expects the {nameof(configurationProvider)} to have defined the Queue Name as {{FIXIT-NMS-ANH-NAME}} ");
      }

      _notificationInstallationMediator = notificationInstallationMediator ?? throw new ArgumentNullException($"{nameof(OnQueueNotifyUsers)} expects a value for {nameof(notificationInstallationMediator)}... null argument was provided");
      _notificationHubClient = NotificationHubClient.CreateClientFromConnectionString(_anhConnectionString, _anhName);
    }

		[FunctionName("OnQueueNotifyUsers")]
		public async Task<IActionResult> RunAsync([QueueTrigger("notificationsqueue", Connection = "FIXIT-NMS-STORAGEACCOUNT-CS")] string queuedNotificationMessage, CancellationToken cancellationToken)
		{
			// validate queue message
			NotificationDto notificationMessage = JsonConvert.DeserializeObject<NotificationDto>(queuedNotificationMessage);
			if (notificationMessage == null)
			{
				throw new InvalidOperationException($"{nameof(OnQueueNotifyUsers)} was unable to properly deserialize {nameof(queuedNotificationMessage)}...");
			}

      // convert tags
      IEnumerable<string> tags = notificationMessage.Tags.Select(item => $"{item.Key}:{item.Value}");

      // get userIds (recipientIds)
      IEnumerable<Guid> recipientIds = notificationMessage.Recipients.Select(userSummaryDto => userSummaryDto.Id);

      // get device installations
      var deviceInstallations = await _notificationInstallationMediator.GetInstallationsAsync(cancellationToken, userIds:recipientIds);

      NotificationOutcome notificationOutcome = null;
      OperationStatus operationStatus = new OperationStatus { IsOperationSuccessful = true };

      foreach (var deviceIntallation in deviceInstallations)
			{
        switch (deviceIntallation.Platform)
        {
          case NotificationPlatform.Wns:
            // Windows 8.1 / Windows Phone 8.1
            var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                        queuedNotificationMessage + "</text></binding></visual></toast>";
            notificationOutcome = await _notificationHubClient.SendWindowsNativeNotificationAsync(toast, tags, cancellationToken);
            break;
          case NotificationPlatform.Apns:
            // iOS
            var alert = "{\"aps\":{\"alert\":\"" + queuedNotificationMessage + "\"}}";
            notificationOutcome = await _notificationHubClient.SendAppleNativeNotificationAsync(alert, tags, cancellationToken);
            break;
          case NotificationPlatform.Fcm:
            // Android
            var notif = "{ \"data\" : {\"message\":\"" + queuedNotificationMessage + "\"}}";
            notificationOutcome = await _notificationHubClient.SendFcmNativeNotificationAsync(notif, tags, cancellationToken);
            break;
        }

        // check notification outcome
        if (notificationOutcome == null
            && ((notificationOutcome.State == NotificationOutcomeState.Abandoned)
            || (notificationOutcome.State == NotificationOutcomeState.Unknown)))
        {
          operationStatus.OperationMessage.Concat($"failed to notify userId {deviceIntallation.UserId}. ");
        }
			}

      return new OkObjectResult(operationStatus);
    }
	}
}
