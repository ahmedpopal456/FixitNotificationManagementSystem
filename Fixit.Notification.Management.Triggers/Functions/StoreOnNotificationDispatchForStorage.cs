using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts.Events.EventGrid.Managers;
using Fixit.Notification.Management.Lib;
using Fixit.Notification.Management.Lib.Constants;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Fixit.Notification.Management.Lib.NotificationAssemblyInfo;

namespace Fixit.Notification.Management.Triggers.Functions
{
  public class StoreOnNotificationDispatchForStorage
  {
    private readonly ILogger<StoreOnNotificationDispatchForStorage> _logger;
    private readonly INotificationMediator _notificationMediator;
    private readonly IEventGridTopicServiceClient _onNotificationStoredTopicServiceClient;

    public StoreOnNotificationDispatchForStorage(IConfiguration configurationProvider,
                                                 ILoggerFactory loggerFactory,
                                                 INotificationMediator notificationMediator,
                                                 EventGridTopicServiceClientResolver eventGridTopicServiceClientResolver)
    {
      _logger = loggerFactory.CreateLogger<StoreOnNotificationDispatchForStorage>();
      _notificationMediator = notificationMediator ?? throw new ArgumentNullException($"{nameof(StoreOnNotificationDispatchForStorage)} expects a value for {nameof(notificationMediator)}... null argument was provided");
      _onNotificationStoredTopicServiceClient = eventGridTopicServiceClientResolver(NotificationEvents.OnNotificationStored) ?? throw new ArgumentNullException($"{nameof(StoreOnNotificationDispatchForStorage)} expects an argument for {nameof(eventGridTopicServiceClientResolver)}. Null argumnent was provided.");
    }

    [FunctionName(nameof(StoreOnNotificationDispatchForStorage))]
    public async Task RunAsync([EventGridTrigger] EventGridEvent enqueuedNotificationMessage, CancellationToken cancellationToken)
    {
      await StoreOnNotificationDispatchForStorageAsync(enqueuedNotificationMessage, cancellationToken);
    }

    public async Task StoreOnNotificationDispatchForStorageAsync(EventGridEvent enqueuedNotificationMessage, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      bool isOperationSuccessful = true; 

      NotificationCreateRequestDto notificationCreateRequestDto = JsonConvert.DeserializeObject<NotificationCreateRequestDto>(enqueuedNotificationMessage.Data.ToString());
      _ = notificationCreateRequestDto ?? throw new InvalidOperationException($"{nameof(StoreOnNotificationDispatchForStorage)} was unable to properly deserialize {nameof(enqueuedNotificationMessage)}...");

      if (!notificationCreateRequestDto.IsTransient)
      {
        var createNotificationResponse = await _notificationMediator.CreateNotificationAsync(notificationCreateRequestDto, cancellationToken);
        isOperationSuccessful = createNotificationResponse.IsOperationSuccessful;
      }

      if(isOperationSuccessful)
      {
        var enqueueNotificationRequestNotification = new EventGridEvent()
        {
          EventTime = DateTime.UtcNow,
          DataVersion = NotificationAssemblyInfo.DataVersion,
          Subject = nameof(StoreOnNotificationDispatchForStorageAsync),
          EventType = nameof(StoreOnNotificationDispatchForStorageAsync),
          Id = Guid.NewGuid().ToString(),
          Data = notificationCreateRequestDto
        };

        var publishNotificationEventsResponse = await _onNotificationStoredTopicServiceClient.PublishEventsToTopicAsync(new List<EventGridEvent> { enqueueNotificationRequestNotification }, cancellationToken);
        if (!publishNotificationEventsResponse.IsOperationSuccessful)
        {
          _logger.LogError($"{nameof(StoreOnNotificationDispatchForStorage)}: Unable to publish {NotificationEvents.OnNotificationStored} event to the {_onNotificationStoredTopicServiceClient}...");
        }
      }
      else
      {
        _logger.LogError($"{nameof(StoreOnNotificationDispatchForStorage)}: Unable to persist notification to the database...");
      }
    }
  }
}