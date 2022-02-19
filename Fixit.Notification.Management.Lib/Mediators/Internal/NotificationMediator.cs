using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.DataContracts;
using Fixit.Core.Storage.Storage.Queue.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[assembly: InternalsVisibleTo("Fixit.Notification.Management.Lib.UnitTests"),
      InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Fixit.Notification.Management.Lib.Mediators.Internal
{
  internal class NotificationMediator : INotificationMediator
  {
    private readonly IMapper _mapper;
    private readonly IQueueClientMediator _notificationsQueue;

    public NotificationMediator(IMapper mapper,
                  IQueueServiceClientMediator queueServiceClientMediator,
                  IConfiguration configuration)
    {
      var notificationQueueName = configuration["FIXIT-NMS-QUEUE-NAME"];

      if (queueServiceClientMediator == null)
      {
        throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(queueServiceClientMediator)}... null argument was provided");
      }

      if (string.IsNullOrWhiteSpace(notificationQueueName))
      {
        throw new ArgumentNullException($"{nameof(NotificationMediator)} expects the {nameof(configuration)} to have defined the Notifications Queue Name as {{FIXIT-NMS-QUEUE-NAME}} ");
      }

      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(mapper)}... null argument was provided");
      _notificationsQueue = queueServiceClientMediator.GetQueueClient(notificationQueueName);

    }

    internal NotificationMediator(IMapper mapper,
                   IQueueServiceClientMediator queueServiceClientMediator,
                   string notificationQueueName)
    {
      if (queueServiceClientMediator == null)
      {
        throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(queueServiceClientMediator)}... null argument was provided");
      }

      if (string.IsNullOrWhiteSpace(notificationQueueName))
      {
        throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(notificationQueueName)}... null argument was provided");
      }

      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(mapper)}... null argument was provided");
      _notificationsQueue = queueServiceClientMediator.GetQueueClient(notificationQueueName);
    }

    #region Enqueue Notification

    public async Task<OperationStatus> EnqueueNotificationAsync(EnqueueNotificationRequestDto enqueueNotificationRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var notificationDto = _mapper.Map<EnqueueNotificationRequestDto, NotificationDto>(enqueueNotificationRequestDto);

      notificationDto.CreatedTimestampUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
      enqueueNotificationRequestDto.Payload = enqueueNotificationRequestDto.Payload != null ? enqueueNotificationRequestDto.Payload: null;

      var operationStatus = await _notificationsQueue.SendMessageAsync(JsonConvert.SerializeObject(notificationDto, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }), TimeSpan.FromSeconds(0), TimeSpan.FromDays(7), cancellationToken);

      return operationStatus;
    }

    #endregion
  }
}