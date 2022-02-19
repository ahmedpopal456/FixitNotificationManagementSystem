using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers;
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
      NotificationDto notificationDto = JsonConvert.DeserializeObject<NotificationDto>(queuedNotificationMessage);
      if (notificationDto == null)
      {
        throw new InvalidOperationException($"{nameof(OnQueueNotifyUsers)} was unable to properly deserialize {nameof(queuedNotificationMessage)}...");
      }

      // get tags and userIds as part of overall tags
      IEnumerable<string> tags = notificationDto.Tags.Select(item => $"{item.Key}:{item.Value}").ToList();
      IEnumerable<Guid> recipientIds = notificationDto.Recipients.Select(UserShortDto => UserShortDto.Id).ToList();

      var targetRecipients = tags.Union(recipientIds.Select(recipientId => $"{UserPrefix}:{recipientId}"));

      var alert = NotificationPayloadConverter.Serialize(notificationDto, NotificationPlatform.Fcm);
      var notif = NotificationPayloadConverter.Serialize(notificationDto, NotificationPlatform.Apns);
      var sendNotificationTasks = new List<Task>
      {
       NotifyAndLogError(() => _notificationHubClient.SendFcmNativeNotificationAsync(alert, targetRecipients, cancellationToken), NotificationPlatform.Fcm),
       NotifyAndLogError(() => _notificationHubClient.SendAppleNativeNotificationAsync(notif, targetRecipients, cancellationToken), NotificationPlatform.Apns),
      };

      await Task.WhenAll(sendNotificationTasks);

      return taskComplete;
    }

    private async Task<NotificationOutcome> NotifyAndLogError(Func<Task<NotificationOutcome>> notify, NotificationPlatform notificationPlatform)
    {
      NotificationOutcome notificationOutcome = null;
      try
      {
        notificationOutcome = await notify();

        // check notification outcome
        if ((notificationOutcome.State == NotificationOutcomeState.Abandoned) ||
         (notificationOutcome.State == NotificationOutcomeState.Unknown))
        {
          var errorMessage = $"{nameof(OnQueueNotifyUsers)} failed to notify devices with platform {notificationPlatform}";
          _logger.LogError(errorMessage);
        }
      }
      catch (Exception)
      {
        // Fall through for the other NotificationOutcomeState type
      }

      return notificationOutcome;
    }
  }
}
