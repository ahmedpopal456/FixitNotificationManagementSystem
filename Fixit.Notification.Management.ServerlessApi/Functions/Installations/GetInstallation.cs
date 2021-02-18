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

namespace Fixit.Notification.Management.ServerlessApi.Functions.Installations
{
  public class GetInstallation
  {    
    private readonly INotificationInstallationMediator _notificationInstallationMediator;
    private readonly IMapper _mapper;

    public GetInstallation(INotificationInstallationMediator notificationInstallationMediator,
                           IMapper mapper)
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(GetInstallation)} expects a value for {nameof(mapper)}... null argument was provided");
      _notificationInstallationMediator = notificationInstallationMediator ?? throw new ArgumentNullException($"{nameof(GetInstallation)} expects a value for {nameof(notificationInstallationMediator)}... null argument was provided");
    }

    [FunctionName(nameof(GetInstallation))]
    [OpenApiOperation("get", "Notifications")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"get", Route = "Notifications/Installations/{id}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         string id)
    {
      return await GetInstallationAsync(id, cancellationToken);
    }

    public async Task<IActionResult> GetInstallationAsync(string installationId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();

      if (string.IsNullOrWhiteSpace(installationId))
      {
        return new BadRequestObjectResult($"{nameof(installationId)} is not a valid {nameof(String)}..");
      }

      var result = await _notificationInstallationMediator.GetInstallationByIdAsync(installationId, cancellationToken);
      if (result == null)
      {
        return new NotFoundObjectResult($"A device installation with id {installationId} was not found");
      }

      return new OkObjectResult(result); 
    }
  }
}
