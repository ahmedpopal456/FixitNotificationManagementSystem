using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Linq;
using Fixit.Notification.Management.Lib.Mediators;

namespace Fixit.Notification.Management.ServerlessApi.Functions.Notifications
{
  public class DeleteNotifications
  {
    private readonly INotificationMediator _notificationMediator;

    public DeleteNotifications(INotificationMediator notificationMediator) : base()
    {
      _notificationMediator = notificationMediator ?? throw new ArgumentNullException($"{nameof(DeleteNotifications)} expects a value for {nameof(notificationMediator)}... null argument was provided");
    }

    [FunctionName(nameof(DeleteNotifications))]
    [OpenApiOperation("delete", "Notifications")]
    [OpenApiParameter("notificationIds", In = ParameterLocation.Header, Required = false, Type = typeof(IEnumerable<string>))]
    [OpenApiParameter("entityId", In = ParameterLocation.Path, Required = true, Type = typeof(long))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"delete", Route = "Notifications/Entity/{entityId:Guid}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid entityId)
    {
      var notificationIds = default(IEnumerable<string>);

      var notificationIdstring = httpRequest.Headers.GetValues("notificationids")?.FirstOrDefault();
      if (!string.IsNullOrWhiteSpace(notificationIdstring))
      {
        notificationIds = notificationIdstring.Split(',');
      }

      return await DeleteNotificationsAsync(httpRequest, notificationIds, entityId, cancellationToken);
    }

    public async Task<IActionResult> DeleteNotificationsAsync(HttpRequestMessage httpRequestMessage, IEnumerable<string> notificationIds, Guid entityId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (notificationIds is null || !notificationIds.All(item => Guid.TryParse(item, out Guid result)))
      {
        return new BadRequestObjectResult($"{nameof(notificationIds)} is not a valid list of {nameof(Guid)}..");
      }

      if (entityId == Guid.Empty)
      {
        return new BadRequestObjectResult($"{nameof(entityId)} is not a valid {nameof(Guid)}..");
      }

      var result = await _notificationMediator.DeleteNotificationsByBulkIdsAsync(entityId.ToString(), notificationIds, cancellationToken);
      return new OkObjectResult(result);
    }
  }
}
