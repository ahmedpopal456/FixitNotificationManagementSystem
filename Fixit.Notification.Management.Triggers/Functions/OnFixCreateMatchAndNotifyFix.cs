using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Models.Notifications.Enums;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Notification.Management.Lib.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Fixit.Core.DataContracts.Notifications.Payloads;
using AutoMapper;

namespace Fixit.Notification.Management.Triggers.Functions
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
        public async Task RunAsync([CosmosDBTrigger(databaseName: "fixit",
                                                    collectionName: "Fixes",
                                                    ConnectionStringSetting = "FIXIT-FMS-DB-CS",
                                                    LeaseCollectionName = "leases",
                                                    LeaseCollectionPrefix = "matchandnotify",
                                                    CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> rawFixDocuments, CancellationToken cancellationToken)
        {
            await MatchAndNotifyFix(rawFixDocuments, cancellationToken);
        }

        public async Task<int> MatchAndNotifyFix(IReadOnlyList<Document> rawFixDocuments, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int taskComplete = 1;
            var exceptions = new ConcurrentQueue<ApplicationException>();

            Parallel.ForEach(rawFixDocuments, async rawFixDocument =>
            {
                var fixDocument = JsonConvert.DeserializeObject<FixDocument>(rawFixDocument.ToString());

                // verify if document new
                if (fixDocument.CreatedTimestampUtc.Equals(fixDocument.UpdatedTimestampUtc))
                {
                    // Get qualified craftsmen list 
                    var enqueueNotificationRequestDto = new EnqueueNotificationRequestDto() { Recipients = await _fixClassificationMediator.GetMinimalQualitifedCraftmen(fixDocument, cancellationToken) };

                    // specify action type 
                    enqueueNotificationRequestDto.Payload = _mapper.Map<FixDocument, FixAssignmentValidationDto>(fixDocument);
                    enqueueNotificationRequestDto.Action = NotificationTypes.FixClientRequest;

                    // enqueue notification
                    var operationStatus = await _notificationMediator.EnqueueNotificationAsync(enqueueNotificationRequestDto, cancellationToken);
                    if (!operationStatus.IsOperationSuccessful)
                    {
                        var errorMessage = $"{nameof(OnFixCreateMatchAndNotifyFix)} failed to enqueue fix with id {fixDocument.id} with message {operationStatus.OperationException}";
                        _logger.LogError(errorMessage);
                        exceptions.Enqueue(new ApplicationException(errorMessage));
                    }
                }
            });

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            return taskComplete;
        }
    }
}
