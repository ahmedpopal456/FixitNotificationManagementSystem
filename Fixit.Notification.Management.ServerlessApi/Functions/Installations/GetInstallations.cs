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

namespace Fixit.Notification.Management.ServerlessApi.Functions.Installations
{
  public class GetInstallations
  {    
    private readonly INotificationInstallationMediator _notificationInstallationMediator;
    private readonly IMapper _mapper;

    public GetInstallations(INotificationInstallationMediator notificationInstallationMediator,
                            IMapper mapper)
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(GetInstallations)} expects a value for {nameof(mapper)}... null argument was provided");
      _notificationInstallationMediator = notificationInstallationMediator ?? throw new ArgumentNullException($"{nameof(GetInstallations)} expects a value for {nameof(notificationInstallationMediator)}... null argument was provided");
    }

    [FunctionName(nameof(GetInstallations))]
    [OpenApiOperation("post", "Notifications")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "Notifications/Installations/Filters")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken)
    {
      return await GetInstallationsAsync(httpRequest, cancellationToken);
    }

    public async Task<IActionResult> GetInstallationsAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (!FixitNotificationsDtoValidators.IsValidDeviceInstallationGetRequest(httpRequestMessage.Content, out DeviceInstallationGetRequest deviceInstallationGetRequest))
      {
        return new BadRequestObjectResult($"Either {nameof(deviceInstallationGetRequest)} is null or has one or more invalid fields...");
      }

      var results = await _notificationInstallationMediator.GetInstallationsAsync(cancellationToken, deviceInstallationGetRequest?.PlatformType, deviceInstallationGetRequest?.Tags, deviceInstallationGetRequest?.UserIds);
      
      return new OkObjectResult(results); 
    }
  }
}
