using System;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Fixit.Core.DataContracts.Notifications.Payloads;
using AutoMapper;
using Fixit.Core.DataContracts.Notifications.Operations;
using Fixit.Core.DataContracts.Notifications.Enums;
using System.Linq;

namespace Fixit.Notification.Management.Triggers.Functions.Matching
{
  public class OnFixCreateMatchAndNotifyFix
  {
    private readonly ILogger _logger;
    private readonly INotificationMediator _notificationMediator;
    private readonly IFixClassificationMediator _fixClassificationMediator;
    private readonly IMapper _mapper;

    public OnFixCreateMatchAndNotifyFix(IMapper mapper,
                                        IConfiguration configurationProvider,
                                        ILoggerFactory loggerFactory,
                                        INotificationMediator notificationMediator,
                                        IFixClassificationMediator fixClassificationMediator)
    {
      _mapper = mapper ?? throw new ArgumentNullException($"{nameof(OnFixCreateMatchAndNotifyFix)} expects a value for {nameof(mapper)}... null argument was provided");
      _logger = loggerFactory.CreateLogger<OnFixCreateMatchAndNotifyFix>();
      _notificationMediator = notificationMediator ?? throw new ArgumentNullException($"{nameof(OnFixCreateMatchAndNotifyFix)} expects a value for {nameof(notificationMediator)}... null argument was provided");
      _fixClassificationMediator = fixClassificationMediator ?? throw new ArgumentNullException($"{nameof(OnFixCreateMatchAndNotifyFix)} expects a value for {nameof(fixClassificationMediator)}... null argument was provided");
    }

    [FunctionName("OnFixCreateMatchAndNotifyFix")]
    public async Task RunAsync([QueueTrigger("fixit-dev-fms-queue", Connection = "FIXIT-FMS-STORAGEACCOUNT-CS")] string queuedOnFixCreateMessage, CancellationToken cancellationToken)
    {
      await MatchAndNotifyFix(queuedOnFixCreateMessage, cancellationToken);
    }

    public async Task<int> MatchAndNotifyFix(string queuedOnFixCreateMessage, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      int taskComplete = 1;
      var fixDocument = JsonConvert.DeserializeObject<FixDocument>(queuedOnFixCreateMessage);

      // verify if document new
      if (fixDocument.CreatedTimestampUtc.Equals(fixDocument.UpdatedTimestampUtc))
      {
        // Get qualified craftsmen list 
        var enqueueNotificationRequestDto = new EnqueueNotificationRequestDto() 
        {
          Title = "Knock Knock",
          Message = "Incoming Request from Client",
          RecipientUsers = await _fixClassificationMediator.GetMinimalQualifiedCraftsmen(fixDocument, cancellationToken),
        };

        if(enqueueNotificationRequestDto.RecipientUsers is { } && enqueueNotificationRequestDto.RecipientUsers.Any())
        {
          // specify action type 
          var fixAssignmentValidationDto = _mapper.Map<FixDocument, FixAssignmentValidationDto>(fixDocument);
          enqueueNotificationRequestDto.Payload = new NotificationPayloadDto() 
          {
            Action = NotificationTypes.FixClientRequest,
            SystemPayload = fixAssignmentValidationDto 
          };

          // enqueue notification
          var operationStatus = await _notificationMediator.EnqueueNotificationAsync(enqueueNotificationRequestDto, cancellationToken);
          if (!operationStatus.IsOperationSuccessful)
          {
            var errorMessage = $"{nameof(OnFixCreateMatchAndNotifyFix)} failed to enqueue fix with id {fixDocument.id} with message {operationStatus.OperationException}";
            _logger.LogError(errorMessage);
          }
        }
      }

      return taskComplete;
    }
  }
}
