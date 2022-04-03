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
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Notification.Management.ServerlessApi.Utils.Validation;

namespace Fixit.Notification.Management.ServerlessApi.Functions.Notifications
{
  public class UpdateNotificationsStatus
  {
    private readonly INotificationMediator _notificationMediator;

    public UpdateNotificationsStatus(INotificationMediator notificationMediator) : base()
    {
      _notificationMediator = notificationMediator ?? throw new ArgumentNullException($"{nameof(UpdateNotificationsStatus)} expects a value for {nameof(notificationMediator)}... null argument was provided");
    }

    [FunctionName(nameof(UpdateNotificationsStatus))]
    [OpenApiOperation("put", "Notifications")]
    [OpenApiRequestBody("application/json", typeof(IEnumerable<NotificationStatusUpdateRequestDto>), Required = true)]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"put", Route = "Notifications/Status")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken)
    {
      return await UpdateNotificationsStatusAsync(httpRequest, cancellationToken);
    }

    public async Task<IActionResult> UpdateNotificationsStatusAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (!EmpowerNotificationsDtoValidators.IsValidNotificationsUpdateStatusRequest(httpRequestMessage.Content, out IEnumerable<NotificationStatusUpdateRequestDto> notificationStatusUpdateRequestDtos))
      {
        return new BadRequestObjectResult($"Either {nameof(notificationStatusUpdateRequestDtos)} is null or has one or more invalid fields...");
      }

      var result = await _notificationMediator.UpdateNotificationsStatusByBulkIdsAsync(notificationStatusUpdateRequestDtos, cancellationToken);
      return new OkObjectResult(result);
    }
  }
}
