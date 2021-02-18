using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Decorators.Exceptions;
using Fixit.Core.Storage.Queue.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Mediators.Internal
{
  internal class NotificationMediator : INotificationMediator
  {
    private readonly IMapper _mapper;
    private readonly INotificationHubClient _notificationHubClient;
    private readonly IQueueClientMediator _notificationsQueue;
    private readonly IExceptionDecorator<OperationStatus> _exceptionDecorator;

    public NotificationMediator(IMapper mapper,
                                INotificationHubClient notificationHubClient,
                                IQueueServiceClientMediator queueServiceClientMediator,
                                IConfiguration configuration,
                                IExceptionDecorator<OperationStatus> exceptionDecorator)
    {
      var cancellationToken = new CancellationTokenSource().Token;
      var notificationQueueName = configuration["FIXIT-NMS-QUEUE-NAME"];

      if (queueServiceClientMediator == null)
      {
        throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(queueServiceClientMediator)}... null argument was provided");
      }

      if (string.IsNullOrWhiteSpace(notificationQueueName))
      {
        throw new ArgumentNullException($"{nameof(NotificationMediator)} expects the {nameof(configuration)} to have defined the Notifications Queue Name as {{FIXIT-NMS-QUEUE-NAME}} ");
      }

      _exceptionDecorator = exceptionDecorator ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(notificationHubClient)}... null argument was provided");
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(mapper)}... null argument was provided");
      _notificationHubClient = notificationHubClient ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(notificationHubClient)}... null argument was provided");
      _notificationsQueue = queueServiceClientMediator.GetQueueClient(notificationQueueName);

    }

    internal NotificationMediator(IMapper mapper,
                                  INotificationHubClient notificationHubClient,
                                  IQueueServiceClientMediator queueServiceClientMediator,
                                  IExceptionDecorator<OperationStatus> exceptionDecorator,
                                  string notificationQueueName)
    {
      var cancellationToken = new CancellationTokenSource().Token;

      if (queueServiceClientMediator == null)
      {
        throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(queueServiceClientMediator)}... null argument was provided");
      }

      if (string.IsNullOrWhiteSpace(notificationQueueName))
      {
        throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(notificationQueueName)}... null argument was provided");
      }

      _exceptionDecorator = exceptionDecorator ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(notificationHubClient)}... null argument was provided");
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(mapper)}... null argument was provided");
      _notificationHubClient = notificationHubClient ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(notificationHubClient)}... null argument was provided");
      _notificationsQueue = queueServiceClientMediator.GetQueueClient(notificationQueueName);
    }

    #region Enqueue Notification

    public async Task<OperationStatus> EnqueueNotificationAsync(EnqueueNotificationRequestDto enqueueNotificationRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var notificationDto = _mapper.Map<EnqueueNotificationRequestDto, NotificationDto>(enqueueNotificationRequestDto);

      // define enqueued time
      notificationDto.CreatedTimestampUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

      // serialize message
      string notificationJson = JsonConvert.SerializeObject(notificationDto);
      string base64EncodedNotification = Convert.ToBase64String(Encoding.UTF8.GetBytes(notificationJson));

      // enqueue notification
      var operationStatus = await _notificationsQueue.SendMessageAsync(base64EncodedNotification, TimeSpan.FromSeconds(0), TimeSpan.FromDays(7), cancellationToken);

      return operationStatus;
    }

    #endregion
  }
}