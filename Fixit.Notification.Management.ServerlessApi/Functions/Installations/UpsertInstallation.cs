using System;
using System.Threading;
using System.Net.Http;
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

namespace Fixit.Notification.Management.ServerlessApi.Functions.Installations
{
  public class UpsertInstallation
  {    
    private readonly INotificationInstallationMediator _notificationInstallationMediator;
    private readonly IMapper _mapper;

    public UpsertInstallation(INotificationInstallationMediator notificationInstallationMediator,
                                      IMapper mapper) : base()
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UpsertInstallation)} expects a value for {nameof(mapper)}... null argument was provided");
      _notificationInstallationMediator = notificationInstallationMediator ?? throw new ArgumentNullException($"{nameof(UpsertInstallation)} expects a value for {nameof(notificationInstallationMediator)}... null argument was provided");
    }

    [FunctionName(nameof(UpsertInstallation))]
    [OpenApiOperation("put", "Notifications")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"put", Route = "Notifications/Installations")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken)
    {
      return await UpsertInstallationAsync(httpRequest, cancellationToken);
    }

    public async Task<IActionResult> UpsertInstallationAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (!FixitNotificationsDtoValidators.IsValidDeviceInstallationUpsertRequest(httpRequestMessage.Content, out DeviceInstallationUpsertRequestDto deviceInstallationUpsertRequestDto))
      {
        return new BadRequestObjectResult($"Either {nameof(deviceInstallationUpsertRequestDto)} is null or has one or more invalid fields...");
      }

      var result = await _notificationInstallationMediator.UpsertInstallationAsync(deviceInstallationUpsertRequestDto, cancellationToken);
      return new OkObjectResult(result); 
    }
  }
}
