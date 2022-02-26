using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Fixit.Notification.Management.Lib.Mediators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi.Models;

namespace Fixit.Notification.Management.ServerlessApi.Functions.Notifications
{
  public class GetNotificationsByPage
  {
    private readonly INotificationMediator _notificationMediator;

    public GetNotificationsByPage(INotificationMediator notificationMediator) : base()
    {
      _notificationMediator = notificationMediator ?? throw new ArgumentNullException($"{nameof(GetNotificationsByPage)} expects a value for {nameof(notificationMediator)}... null argument was provided");
    }

    [FunctionName(nameof(GetNotificationsByPage))]
    [OpenApiOperation("get", "Notifications")]
    [OpenApiParameter("pageNumber", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
    [OpenApiParameter("countPerPage", In = ParameterLocation.Path, Required = true, Type = typeof(int))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"get", Route = "notifications/entity/{entityId:Guid}/pages/{pageNumber:int}/{countPerPage:int}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         Guid entityId,
                                         int pageNumber,
                                         int countPerPage)
    {
      return await GetNotificationsByPageAsync(entityId, pageNumber, countPerPage, cancellationToken);
    }

    public async Task<IActionResult> GetNotificationsByPageAsync(Guid entityId, int pageNumber, int countPerPage, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (pageNumber <= default(long))
      {
        return new BadRequestObjectResult($"{nameof(pageNumber)} is not valid; value provided is either equal or less than 0...");
      }
      if (countPerPage <= default(long))
      {
        return new BadRequestObjectResult($"{nameof(countPerPage)} is not valid; value provided is either equal or less than 0...");
      }

      var result = await _notificationMediator.GetNotificationsByPageAsync(entityId.ToString(), pageNumber, countPerPage, cancellationToken);
      return new OkObjectResult(result);
    }
  }
}
