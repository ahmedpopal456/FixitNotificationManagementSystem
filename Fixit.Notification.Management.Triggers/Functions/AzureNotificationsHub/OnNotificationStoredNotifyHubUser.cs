using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Notification.Management.Lib.Resolvers;
using Fixit.Notification.Management.Triggers.Functions.Utils;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Triggers.Functions.AzureNotificationsHub
{
  public class OnNotificationStoredNotifyHubUser
  {
    private readonly string _userPrefix;

    private readonly ILogger _logger;
    private readonly INotificationHubClient _notificationHubClient;

    public OnNotificationStoredNotifyHubUser(ILoggerFactory loggerFactory,
                                             IConfiguration configurationProvider,
                                             INotificationHubClient notificationHubClient)
    {
      var userPrefix = configurationProvider["UserTagPrefix"];

      if (string.IsNullOrWhiteSpace(userPrefix))
      {
        throw new ArgumentNullException($"{nameof(OnNotificationStoredNotifyHubUser)} expects the {nameof(configurationProvider)} to have defined the User Prefix Name as {{UserTagPrefix}} ");
      }
      
      _logger = loggerFactory.CreateLogger<OnNotificationStoredNotifyHubUser>();
      _userPrefix = userPrefix;
      _notificationHubClient = notificationHubClient ?? throw new ArgumentNullException($"{nameof(OnNotificationStoredNotifyHubUser)} expects a value for {nameof(notificationHubClient)}... null argument was provided");
    }

    [FunctionName(nameof(OnNotificationStoredNotifyHubUser))]
    public async Task RunAsync([EventGridTrigger] EventGridEvent queuedNotificationMessage, CancellationToken cancellationToken)
    {
      await NotifyUser(queuedNotificationMessage, cancellationToken);
    }

    public async Task NotifyUser(EventGridEvent queuedNotificationMessage, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      NotificationCreateRequestDto notificationCreateRequestDto = JsonConvert.DeserializeObject<NotificationCreateRequestDto>(queuedNotificationMessage.Data.ToString());

      _ = notificationCreateRequestDto ?? throw new InvalidOperationException($"{nameof(OnNotificationStoredNotifyHubUser)} was unable to properly deserialize {nameof(queuedNotificationMessage)}...");

      IEnumerable<string> tag = new List<string> { $"{_userPrefix}:{notificationCreateRequestDto.RecipientUser.Id}" };

      var apnsPayload = NotificationPayloadConverter.Serialize(notificationCreateRequestDto, NotificationPlatform.Apns);
      var fcmPayload =  NotificationPayloadConverter.Serialize(notificationCreateRequestDto, NotificationPlatform.Fcm);
      
      var sendNotificationTasks = new List<Task>
      {
        NotificationFunctionUtils.NotifyAndLogError(() => _notificationHubClient.SendAppleNativeNotificationAsync(apnsPayload, tag, cancellationToken), NotificationPlatform.Apns, _logger),
        NotificationFunctionUtils.NotifyAndLogError(() => _notificationHubClient.SendFcmNativeNotificationAsync(fcmPayload, tag, cancellationToken), NotificationPlatform.Fcm, _logger)
      };

      await Task.WhenAll(sendNotificationTasks);
    }
  }
}