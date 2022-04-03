using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.Database.DataContracts.Documents;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Notifications.Operations;
using Fixit.Notification.Management.Lib.Models.Notifications;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Responses;

namespace Fixit.Notification.Management.Lib.Mediators
{
  public interface INotificationMediator
  {
    /// <summary>
    /// Enqueue notification request
    /// </summary>
    /// <param name="enqueueNotificationRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<OperationStatus> EnqueueNotificationAsync(EnqueueNotificationRequestDto enqueueNotificationRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// Dispatch notifications for creation.
    /// </summary>
    /// <param name="notificationCreateRequestDtos"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DispatchNotificationsForCreationAsync(IEnumerable<NotificationCreateRequestDto> notificationCreateRequestDtos, CancellationToken cancellationToken);

    /// <summary>
    /// Create notifications, asynchronously.
    /// </summary>
    /// <param name="notificationDocument"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<NotificationsCreateResponse> CreateNotificationAsync(NotificationCreateRequestDto notificationCreateRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// Update a notification's status. 
    /// </summary>
    /// <param name="notificationStatusUpdateRequestDtos"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<OperationStatusWithObject<NotificationStatusUpdateResponseDto>>> UpdateNotificationsStatusByBulkIdsAsync(IEnumerable<NotificationStatusUpdateRequestDto> notificationStatusUpdateRequestDtos, CancellationToken cancellationToken);

    /// <summary>
    /// Get notifications by page and by user. 
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="currentPage"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PagedDocumentCollectionDto<NotificationDocument>> GetNotificationsByPageAsync(string entityId, int currentPage, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Delete notifications for a given user, by id. 
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="notificationIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<OperationStatusWithObject<NotificationsDeleteResponse>>> DeleteNotificationsByBulkIdsAsync(string entityId, IEnumerable<string> notificationIds, CancellationToken cancellationToken);
  }
}
