using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

    public OnQueueNotifyUsers(IConfiguration configurationProvider,
                              INotificationInstallationMediator notificationInstallationMediator,
                              INotificationHubClient notificationHubClient)
		{

      _notificationInstallationMediator = notificationInstallationMediator ?? throw new ArgumentNullException($"{nameof(OnQueueNotifyUsers)} expects a value for {nameof(notificationInstallationMediator)}... null argument was provided");
      _notificationHubClient = notificationHubClient ?? throw new ArgumentNullException($"{nameof(OnQueueNotifyUsers)} expects a value for {nameof(notificationHubClient)}... null argument was provided");
    }

    [FunctionName("OnQueueNotifyUsers")]
		public async Task RunAsync([QueueTrigger("notificationsqueue", Connection = "FIXIT-NMS-STORAGEACCOUNT-CS")] string queuedNotificationMessage, ILogger logger, CancellationToken cancellationToken)
		{
      await NotifyUsers(queuedNotificationMessage, logger, cancellationToken);
    }

    public async Task NotifyUsers(string queuedNotificationMessage, ILogger logger, CancellationToken cancellationToken)
    {
      // validate queue message
      NotificationDto notificationMessage = JsonConvert.DeserializeObject<NotificationDto>(queuedNotificationMessage);
      if (notificationMessage == null)
      {
        throw new InvalidOperationException($"{nameof(OnQueueNotifyUsers)} was unable to properly deserialize {nameof(queuedNotificationMessage)}...");
      }

      // convert tags
      IEnumerable<string> tags = notificationMessage.Tags.Select(item => $"{item.Key}:{item.Value}").ToList();

      // get userIds (recipientIds)
      IEnumerable<Guid> recipientIds = notificationMessage.Recipients.Select(userSummaryDto => userSummaryDto.Id).ToList();

      // get device installations
      var deviceInstallations = await _notificationInstallationMediator.GetInstallationsAsync(cancellationToken, userIds: recipientIds);

      // serialize notification message
      string notificationJson = JsonConvert.SerializeObject(notificationMessage);
      string base64EncodedMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(notificationJson));
      byte[] byteArrayMessage = Convert.FromBase64String(base64EncodedMessage);

      NotificationOutcome notificationOutcome = null;
      foreach (var deviceIntallation in deviceInstallations)
      {
        switch (deviceIntallation.Platform)
        {
          case NotificationPlatform.Wns:
            // Windows 8.1 / Windows Phone 8.1
            var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                        byteArrayMessage + "</text></binding></visual></toast>";
            notificationOutcome = await _notificationHubClient.SendWindowsNativeNotificationAsync(toast, tags, cancellationToken);
            break;
          case NotificationPlatform.Apns:
            // iOS
            var alert = "{\"aps\":{\"alert\":\"" + byteArrayMessage + "\"}}";
            notificationOutcome = await _notificationHubClient.SendAppleNativeNotificationAsync(alert, tags, cancellationToken);
            break;
          case NotificationPlatform.Fcm:
            // Android
            var notif = "{ \"data\" : {\"message\":\"" + byteArrayMessage + "\"}}";
            notificationOutcome = await _notificationHubClient.SendFcmNativeNotificationAsync(notif, tags, cancellationToken);
            break;
        }

        // check notification outcome
        if (notificationOutcome == null
            && ((notificationOutcome.State == NotificationOutcomeState.Abandoned)
            || (notificationOutcome.State == NotificationOutcomeState.Unknown)))
        {
          logger.LogError($"Failed to notify user id {deviceIntallation.UserId} with device id {deviceIntallation.InstallationId} and notification outcoume {notificationOutcome?.State}...");
        }
      }
    }
	}
}
