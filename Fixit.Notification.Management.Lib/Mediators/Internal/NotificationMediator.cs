using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.Database.DataContracts.Documents;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Events.EventGrid.Managers;
using Fixit.Core.DataContracts.Notifications.Operations;
using Fixit.Core.Storage.Storage.Queue.Mediators;
using Fixit.Notification.Management.Lib.Constants;
using Fixit.Notification.Management.Lib.Models.Notifications;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Responses;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static Fixit.Notification.Management.Lib.NotificationAssemblyInfo;

[assembly: InternalsVisibleTo("Fixit.Notification.Management.Lib.UnitTests"),
      InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Fixit.Notification.Management.Lib.Mediators.Internal
{
  internal class NotificationMediator : INotificationMediator
  {
    private readonly IMapper _mapper;
    private readonly IQueueClientMediator _notificationsQueue;
    private readonly IEventGridTopicServiceClient _onNotificationDispatchedForStorageTopicServiceClient;
    private readonly IEventGridTopicServiceClient _onNotificationEnqueuedTopicServiceClient;
    private readonly ILogger<NotificationMediator> _logger;
    private readonly IDatabaseTableEntityMediator _notificationsContainer;

    public NotificationMediator(IDatabaseMediator databaseMediator, 
                                IMapper mapper,
                                IQueueServiceClientMediator queueServiceClientMediator,
                                ILogger<NotificationMediator> logger,
                                IConfiguration configuration,
                                EventGridTopicServiceClientResolver eventGridTopicServiceClientResolver)
    {
      var notificationQueueName = configuration["FIXIT-NMS-QUEUE-NAME"];
      var notificationsContainerName = configuration["FIXIT-NMS-DB-NOTIFICATIONS"];
      var databaseName = configuration["FIXIT-NMS-DB-NAME"];

      _ = string.IsNullOrWhiteSpace(databaseName) ? throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects the {nameof(configuration)} to have defined the Database Name as {{EMP-NMS-DB-NAME}}") : string.Empty;
      _ = string.IsNullOrWhiteSpace(notificationsContainerName) ? throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects the {nameof(configuration)} to have defined the Notifications Table as {{EMP-NMS-DB-NOTIFICATIONS}}") : string.Empty;

      _logger = logger ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(logger)}... null argument was provided"); ;
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects a value for {nameof(mapper)}... null argument was provided");
      _notificationsQueue = queueServiceClientMediator.GetQueueClient(notificationQueueName);
      _onNotificationEnqueuedTopicServiceClient = eventGridTopicServiceClientResolver(NotificationEvents.OnNotificationEnqueued) ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects an argument for {nameof(eventGridTopicServiceClientResolver)}. Null argumnent was provided.");
      _onNotificationDispatchedForStorageTopicServiceClient = eventGridTopicServiceClientResolver(NotificationEvents.OnNotificationDispatchedForStorage) ?? throw new ArgumentNullException($"{nameof(NotificationMediator)} expects an argument for {nameof(eventGridTopicServiceClientResolver)}. Null argumnent was provided.");

      var database = databaseMediator.GetDatabase(databaseName);
      _notificationsContainer = database.GetContainer(notificationsContainerName);
    }

    #region Enqueue Notification

    public async Task<OperationStatus> EnqueueNotificationAsync(EnqueueNotificationRequestDto enqueueNotificationRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var enqueueNotificationRequestNotification = new EventGridEvent()
      {
        EventTime = DateTime.UtcNow,
        DataVersion = NotificationAssemblyInfo.DataVersion,
        Subject = nameof(EnqueueNotificationAsync),
        EventType = nameof(EnqueueNotificationAsync),
        Id = Guid.NewGuid().ToString(),
        Data = enqueueNotificationRequestDto
      };

      return await _onNotificationEnqueuedTopicServiceClient.PublishEventsToTopicAsync(new List<EventGridEvent> { enqueueNotificationRequestNotification }, CancellationToken.None);
    }

    public async Task DispatchNotificationsForCreationAsync(IEnumerable<NotificationCreateRequestDto> notificationCreateRequestDtos, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      _ = notificationCreateRequestDtos ?? throw new ArgumentNullException($"{nameof(DispatchNotificationsForCreationAsync)} expects a valid value for {nameof(notificationCreateRequestDtos)}.. null argument was provided...");

      List<Task> dispatchNotificationsForCreationTasks = new List<Task>();
      foreach (var notificationCreateRequestDto in notificationCreateRequestDtos)
      {
        dispatchNotificationsForCreationTasks.Add(Task.Run(async () =>
        {
          var enqueueNotificationRequestNotification = new EventGridEvent()
          {
            EventTime = DateTime.UtcNow,
            DataVersion = NotificationAssemblyInfo.DataVersion,
            Subject = nameof(DispatchNotificationsForCreationAsync),
            EventType = nameof(DispatchNotificationsForCreationAsync),
            Id = notificationCreateRequestDto.Id.ToString(),
            Data = notificationCreateRequestDto
          };

          var publishNotificationEventsResponse = await _onNotificationDispatchedForStorageTopicServiceClient.PublishEventsToTopicAsync(new List<EventGridEvent> { enqueueNotificationRequestNotification }, cancellationToken);
          if (!publishNotificationEventsResponse.IsOperationSuccessful)
          {
            _logger.LogError($"{nameof(DispatchNotificationsForCreationAsync)}: Unable to publish {NotificationEvents.OnNotificationDispatchedForStorage}, to the {_onNotificationDispatchedForStorageTopicServiceClient}...");
          }
        }));
      }
      await Task.WhenAll(dispatchNotificationsForCreationTasks);
    }

    #endregion

    #region Create Notifications

    public async Task<NotificationsCreateResponse> CreateNotificationAsync(NotificationCreateRequestDto notificationCreateRequestDto, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      _ = notificationCreateRequestDto ?? throw new ArgumentNullException($"{nameof(CreateNotificationAsync)} expects a valid value for {nameof(notificationCreateRequestDto)}.. null argument was provided...");

      var notificationDocument = _mapper.Map<NotificationCreateRequestDto, NotificationDocument>(notificationCreateRequestDto);

      var createDocumentResponse = await _notificationsContainer.CreateItemAsync<NotificationDocument>(notificationDocument, notificationDocument.EntityId, cancellationToken);
      var result = new NotificationsCreateResponse(createDocumentResponse) { Notification = createDocumentResponse.IsOperationSuccessful ? notificationDocument : null };
      return result;
    }

    #endregion

    #region Update Notifications 

    public async Task<IEnumerable<OperationStatusWithObject<NotificationStatusUpdateResponseDto>>> UpdateNotificationsStatusByBulkIdsAsync(IEnumerable<NotificationStatusUpdateRequestDto> notificationStatusUpdateRequestDtos, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (notificationStatusUpdateRequestDtos == null || !notificationStatusUpdateRequestDtos.Any())
      {
        throw new ArgumentNullException($"{nameof(UpdateNotificationsStatusByBulkIdsAsync)} expects a valid value for {nameof(notificationStatusUpdateRequestDtos)}.. null or empty list was provided...");
      }

      var results = new BlockingCollection<OperationStatusWithObject<NotificationStatusUpdateResponseDto>>();

      var notificationIds = notificationStatusUpdateRequestDtos.Select(notificationUpdateRequest => notificationUpdateRequest.Id.ToString()).ToList();

      var (documentResponse, token) = await _notificationsContainer.GetItemQueryableAsync<NotificationDocument>(null, cancellationToken, insight => (notificationIds.Contains(insight.id)), null);
      if (documentResponse.IsOperationSuccessful && notificationStatusUpdateRequestDtos.Count() == documentResponse.Results?.Count())
      {
        List<Task> updateInsightTasks = new List<Task>();
        foreach (var notificationDocument in documentResponse.Results)
        {
          var notificationUpdateRequest = notificationStatusUpdateRequestDtos.FirstOrDefault(notificationStatusUpdateRequestDto => notificationStatusUpdateRequestDto.Id.ToString() == notificationDocument.id);
          var currentTimestampUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

          notificationDocument.UpdatedTimestampUtc = currentTimestampUtc;
          notificationDocument.Status = notificationUpdateRequest.Status;

          updateInsightTasks.Add(Task.Run(async () =>
          {
            var result = new OperationStatusWithObject<NotificationStatusUpdateResponseDto>();

            var updateResponse = await _notificationsContainer.UpsertItemAsync<NotificationDocument>(notificationDocument, notificationDocument.EntityId, cancellationToken);
            _mapper.Map<OperationStatus, OperationStatusWithObject<NotificationStatusUpdateResponseDto>>(updateResponse, result);
            result.Result = result.IsOperationSuccessful ? new NotificationStatusUpdateResponseDto() { NotificationId = notificationUpdateRequest.Id } : result.Result;
            results.Add(result);
          }));
        }
        await Task.WhenAll(updateInsightTasks);
      }
      return results;
    }

    #endregion

    #region Delete Notifications 

    public async Task<IEnumerable<OperationStatusWithObject<NotificationsDeleteResponse>>> DeleteNotificationsByBulkIdsAsync(string entityId, IEnumerable<string> notificationIds, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (string.IsNullOrWhiteSpace(entityId) || !Guid.TryParse(entityId, out Guid parsedEntityId))
      {
        throw new ArgumentNullException($"{nameof(DeleteNotificationsByBulkIdsAsync)} expects a valid value for {nameof(entityId)}...");
      }
      if (notificationIds.Any(notificationId => notificationId.Equals(Guid.Empty)))
      {
        throw new ArgumentNullException($"{nameof(DeleteNotificationsByBulkIdsAsync)} expects valid {nameof(Guid)} values for {nameof(notificationIds)}...");
      }

      var results = new List<OperationStatusWithObject<NotificationsDeleteResponse>>();

      var (documentResponse, token) = await _notificationsContainer.GetItemQueryableAsync<NotificationDocument>(null, cancellationToken, insight => (notificationIds.Contains(insight.id)), null);
      if (documentResponse.IsOperationSuccessful && notificationIds.Count() == documentResponse.Results?.Count())
      {
        List<Task> deleteInsightTasks = new List<Task>();
        foreach (var notificationDocument in documentResponse.Results)
        {
          var currentTimestampUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

          notificationDocument.UpdatedTimestampUtc = notificationDocument.DeletedTimestampUtc = currentTimestampUtc;
          notificationDocument.IsDeleted = true;

          deleteInsightTasks.Add(Task.Run(async () =>
          {
            var result = new OperationStatusWithObject<NotificationsDeleteResponse>();

            var updateResponse = await _notificationsContainer.UpsertItemAsync<NotificationDocument>(notificationDocument, notificationDocument.EntityId, cancellationToken);
            _mapper.Map<OperationStatus, OperationStatusWithObject<NotificationsDeleteResponse>>(updateResponse, result);
            result.Result = result.IsOperationSuccessful ? new NotificationsDeleteResponse() { NotificationId = Guid.Parse(notificationDocument.id) } : result.Result;
            results.Add(result);
          }));
        }
        await Task.WhenAll(deleteInsightTasks);
      }
      return results;
    }

    #endregion

    #region Get Notifications

    public async Task<PagedDocumentCollectionDto<NotificationDocument>> GetNotificationsByPageAsync(string entityId, int currentPage, int pageSize, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var result = new PagedDocumentCollectionDto<NotificationDocument>();

      QueryRequestOptions queryRequestOptions = new QueryRequestOptions()
      {
        PartitionKey = new PartitionKey(entityId),
        MaxItemCount = pageSize
      };

      PagedDocumentCollectionDto<NotificationDocument> documentResponse = await _notificationsContainer.GetItemQueryableByPageAsync<NotificationDocument>(currentPage, queryRequestOptions,cancellationToken, (document) => document.IsDeleted == false);
      if (documentResponse != null && documentResponse.IsOperationSuccessful)
      {
        foreach(var document in documentResponse.Results)
        {
          if(document is {Payload:{SystemPayload:{ } systemPayload } } && systemPayload is{ })
            document.Payload.SystemPayload = JsonConvert.SerializeObject(systemPayload, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        result.PageNumber = documentResponse.PageNumber;
        result.Results = documentResponse.Results;
        result.IsOperationSuccessful = documentResponse.IsOperationSuccessful;
      }

      return result;
    }

    #endregion
  }
}