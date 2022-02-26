using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.DataContracts.Notifications.Operations;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Triggers.Functions
{
  public class DispatchForStorageOnNotificationEnqueued
  {
    private readonly IMapper _mapper; 
    private readonly INotificationMediator _notificationMediator;

    public DispatchForStorageOnNotificationEnqueued(IConfiguration configurationProvider,
                                                   ILoggerFactory loggerFactory,
                                                   IMapper mapper, 
                                                   INotificationMediator notificationMediator)
    {
      _notificationMediator = notificationMediator ?? throw new ArgumentNullException($"{nameof(DispatchForStorageOnNotificationEnqueued)} expects a value for {nameof(notificationMediator)}... null argument was provided");
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(DispatchForStorageOnNotificationEnqueued)} expects a value for {nameof(mapper)}... null argument was provided");
    }

    [FunctionName(nameof(DispatchForStorageOnNotificationEnqueued))]
    public async Task RunAsync([EventGridTrigger] EventGridEvent enqueuedNotificationMessage, CancellationToken cancellationToken)
    {
      await DispatchNotificationsForCreationAsync(enqueuedNotificationMessage, cancellationToken);
    }
    public async Task DispatchNotificationsForCreationAsync(EventGridEvent enqueuedNotificationMessage, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      EnqueueNotificationRequestDto enqueueNotificationRequestDto = JsonConvert.DeserializeObject<EnqueueNotificationRequestDto>(enqueuedNotificationMessage.Data.ToString());
      _ = enqueueNotificationRequestDto ?? throw new InvalidOperationException($"{nameof(DispatchForStorageOnNotificationEnqueued)} was unable to properly deserialize {nameof(enqueuedNotificationMessage)}...");      

      var notificationDocumentCreateRequests = _mapper.Map<EnqueueNotificationRequestDto, IList<NotificationCreateRequestDto>>(enqueueNotificationRequestDto);
      await _notificationMediator.DispatchNotificationsForCreationAsync(notificationDocumentCreateRequests, cancellationToken);
    }
  }
}