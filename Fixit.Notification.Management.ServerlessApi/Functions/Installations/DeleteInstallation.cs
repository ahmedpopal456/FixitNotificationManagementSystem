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
  public class DeleteInstallation
  {    
    private readonly INotificationInstallationMediator _notificationInstallationMediator;
    private readonly IMapper _mapper;

    public DeleteInstallation(INotificationInstallationMediator notificationInstallationMediator,
                              IMapper mapper)
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(UpsertInstallation)} expects a value for {nameof(mapper)}... null argument was provided");
      _notificationInstallationMediator = notificationInstallationMediator ?? throw new ArgumentNullException($"{nameof(UpsertInstallation)} expects a value for {nameof(notificationInstallationMediator)}... null argument was provided");
    }

    [FunctionName(nameof(DeleteInstallation))]
    [OpenApiOperation("delete", "Notifications")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,"delete", Route = "Notifications/Installations/{id}")]
                                         HttpRequestMessage httpRequest,
                                         CancellationToken cancellationToken,
                                         string id)
    {
      return await DeleteInstallationAsync(id, cancellationToken);
    }

    public async Task<IActionResult> DeleteInstallationAsync(string installationId, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
     
      if (string.IsNullOrWhiteSpace(installationId))
      {
        return new BadRequestObjectResult($"{nameof(installationId)} is not a valid {nameof(String)}..");
      }

      var result = await _notificationInstallationMediator.DeleteInstallationById(installationId, cancellationToken);
      if(result == null)
      {
        return new NotFoundObjectResult($"A device installation with id {installationId} was not found to delete..");
      }

      return new OkObjectResult(result); 
    }
  }
}
