using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.OpenApi.Models;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Notification.Management.ServerlessApi.Helpers;

namespace Fixit.Notification.Management.ServerlessApi.Functions.Notifications
{
  public class EnqueueNotification
  {
    private readonly INotificationMediator _notificationMediator;
    private readonly IMapper _mapper;

    public EnqueueNotification(INotificationMediator notificationMediator,
                               IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(EnqueueNotification)} expects a value for {nameof(mapper)}... null argument was provided");
      _notificationMediator = notificationMediator ?? throw new ArgumentNullException($"{nameof(EnqueueNotification)} expects a value for {nameof(notificationMediator)}... null argument was provided");
    }

    [FunctionName(nameof(EnqueueNotification))]
    [OpenApiOperation("post", "Notifications")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "Notifications")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken)
    {
      return await EnqueueNotificationAsync(httpRequest, cancellationToken);
    }

    public async Task<IActionResult> EnqueueNotificationAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (!FixitNotificationsDtoValidators.IsValidEnqueueNotificationRequest(httpRequestMessage.Content, out EnqueueNotificationRequestDto enqueueNotificationRequestDto))
      {
        return new BadRequestObjectResult($"Either {nameof(enqueueNotificationRequestDto)} is null or has one or more invalid fields...");
      }

      var result = await _notificationMediator.EnqueueNotificationAsync(enqueueNotificationRequestDto, cancellationToken);
      return new OkObjectResult(result); 
    }
  }
}
