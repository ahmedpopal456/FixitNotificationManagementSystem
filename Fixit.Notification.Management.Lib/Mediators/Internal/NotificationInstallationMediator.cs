using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Decorators.Exceptions;
using Fixit.Notification.Management.Lib.Models.Notifications;
using Fixit.Notification.Management.Lib.Models.Notifications.Installations;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Notification.Management.Lib.Models.Notifications.Templates;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Fixit.Notification.Management.Lib.UnitTests"),
					 InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Fixit.Notification.Management.Lib.Mediators.Internal
{
	internal class NotificationInstallationMediator : INotificationInstallationMediator
	{
		private readonly ILogger<NotificationInstallationMediator> _logger;
		private readonly IMapper _mapper;
		private readonly IDatabaseTableEntityMediator _deviceInstallationContainer;
		private readonly INotificationHubClient _notificationHubClient;
		private readonly IExceptionDecorator<OperationStatus> _exceptionDecorator;
		private readonly string UserPrefix = "userId";

		public NotificationInstallationMediator(IDatabaseMediator databaseMediator,
																						IMapper mapper,
																						INotificationHubClient notificationHubClient,
																						IConfiguration configuration,
																						ILogger<NotificationInstallationMediator> logger,
																						IExceptionDecorator<OperationStatus> exceptionDecorator)
		{
			var databaseName = configuration["FIXIT-NMS-DB-NAME"];
			var deviceInstallationsContainerName = configuration["FIXIT-NMS-DB-INSTALLATIONS"];

			if (databaseMediator == null)
			{
				throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(databaseMediator)}... null argument was provided");
			}

			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects the {nameof(configuration)} to have defined the Database Name as {{EMP-INGT-DB-NAME}} ");
			}

			if (string.IsNullOrWhiteSpace(deviceInstallationsContainerName))
			{
				throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects the {nameof(configuration)} to have defined the Notifications Device Installations Table as {{FIXIT-NMS-DB-INSTALLATION}} ");
			}

			_exceptionDecorator = exceptionDecorator ?? throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(notificationHubClient)}... null argument was provided");
			_notificationHubClient = notificationHubClient ?? throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(notificationHubClient)}... null argument was provided");
			_mapper = mapper ?? throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(mapper)}... null argument was provided");
			_logger = logger ?? throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(logger)}... null argument was provided");

			var database = databaseMediator.GetDatabase(databaseName);
			_deviceInstallationContainer = database.GetContainer(deviceInstallationsContainerName);
		}

		internal NotificationInstallationMediator(IDatabaseMediator databaseMediator,
																							IMapper mapper,
																							INotificationHubClient notificationHubClient,
																							ILogger<NotificationInstallationMediator> logger,
																							IExceptionDecorator<OperationStatus> exceptionDecorator,
																							string databaseName,
																							string deviceInstallationsContainerName)
		{
			if (databaseMediator == null)
			{
				throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(databaseMediator)}... null argument was provided");
			}

			if (string.IsNullOrWhiteSpace(databaseName))
			{
				throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(databaseName)}... null argument was provided");
			}

			if (string.IsNullOrWhiteSpace(deviceInstallationsContainerName))
			{
				throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(deviceInstallationsContainerName)}... null argument was provided");
			}

			_exceptionDecorator = exceptionDecorator ?? throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(notificationHubClient)}... null argument was provided");
			_notificationHubClient = notificationHubClient ?? throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(notificationHubClient)}... null argument was provided");
			_mapper = mapper ?? throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(mapper)}... null argument was provided");
			_logger = logger ?? throw new ArgumentNullException($"{nameof(NotificationInstallationMediator)} expects a value for {nameof(logger)}... null argument was provided");

			var database = databaseMediator.GetDatabase(databaseName);
			_deviceInstallationContainer = database.GetContainer(deviceInstallationsContainerName);
		}

		#region Create or Update Installations

		public async Task<OperationStatus> UpsertInstallationAsync(DeviceInstallationUpsertRequestDto deviceInstallationUpsertRequestDto, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var operationStatus = new OperationStatus { IsOperationSuccessful = true };
			var updateInstallation = _mapper.Map<DeviceInstallationUpsertRequestDto, Installation>(deviceInstallationUpsertRequestDto);

			// add the user's id as a tag 
			updateInstallation.Tags?.Add($"{UserPrefix}:{deviceInstallationUpsertRequestDto.UserId}");

			// add device templates
			updateInstallation.Templates = deviceInstallationUpsertRequestDto.Templates?.Select(template => new { template.Key, Value = _mapper.Map<NotificationTemplateBaseDto, InstallationTemplate>(template.Value) }).ToDictionary(pair => pair.Key, pair => pair.Value);

			// apply device installation
			operationStatus = await _exceptionDecorator.ExecuteOperationAsync(operationStatus, () => _notificationHubClient.CreateOrUpdateInstallationAsync(updateInstallation, cancellationToken));
			if (operationStatus.IsOperationSuccessful)
			{
				var currentTimestampUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

				// map request to device installation document 
				var installationDocument = _mapper.Map<DeviceInstallationUpsertRequestDto, DeviceInstallationDocument>(deviceInstallationUpsertRequestDto);

				installationDocument.InstalledTimestampUtc = currentTimestampUtc;
				installationDocument.UpdatedTimestampUtc = currentTimestampUtc;

				// update installation record
				operationStatus = await _deviceInstallationContainer.UpdateItemAsync<DeviceInstallationDocument>(installationDocument, installationDocument.EntityId, cancellationToken);
			}

			return operationStatus;
		}

		#endregion

		#region Get Installations

		public async Task<DeviceInstallationDto> GetInstallationByIdAsync(string installationId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			DeviceInstallationDto deviceInstallationDto = default;
			Installation installation;

			// get device installation dto
			try
			{
				installation = await _notificationHubClient.GetInstallationAsync(installationId, cancellationToken);
			}
			catch (Exception exception)
			{
				_logger.LogError($"{nameof(NotificationHubClient)} method GetInstallationAsync failed with exception {exception}");
				// for now to force the  500 internal error from Azure Notification Hub
				installation = _notificationHubClient.GetInstallation(installationId);
			}

			if (installation != default)
			{
				// map request to device installation document 
				deviceInstallationDto = _mapper.Map<Installation, DeviceInstallationDto>(installation);

				// get user id from installation
				var userIdTag = installation.Tags?.FirstOrDefault(tag => tag.Split(":", StringSplitOptions.None).FirstOrDefault() == "UserPrefix");
				var userIdString = userIdTag?.Split(":", StringSplitOptions.None).LastOrDefault();

				Guid.TryParse(userIdString, out Guid userId);
				deviceInstallationDto.UserId = userId;
			}

			return deviceInstallationDto;
		}

		public async Task<IEnumerable<DeviceInstallationDto>> GetInstallationsAsync(CancellationToken cancellationToken, NotificationPlatform? platformType = null, IEnumerable<NotificationTagDto> tags = null, IEnumerable<Guid> userIds = null)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var deviceInstallationDtos = new List<DeviceInstallationDto>();

			// define filters
			var userIdsString = userIds.Select(userId => userId.ToString());
			tags ??= new List<NotificationTagDto>();
			Expression<Func<DeviceInstallationDocument, bool>> expression = deviceInstallation => (platformType == null || deviceInstallation.Platform.ToString() == platformType.ToString())
																																														&& (userIds == null || userIdsString.Contains(deviceInstallation.EntityId))
																																														&& (deviceInstallation.Tags.Select(devTag => tags.Contains(devTag)) != null);

			// get all installations based on previous filter
			var deviceInstallations = new List<DeviceInstallationDocument>();

			string continuationToken = "";
			while (continuationToken != null)
			{
				var (documentCollection, tempContinuationToken) = await _deviceInstallationContainer.GetItemQueryableAsync(continuationToken, cancellationToken, expression, null);

				continuationToken = tempContinuationToken;
				if (documentCollection.IsOperationSuccessful)
				{
					deviceInstallations.AddRange(documentCollection.Results);
				}
			}

			// if any installations exist
			if (deviceInstallations?.Any() == true)
			{
				deviceInstallationDtos = deviceInstallations.Select(deviceInstallation => _mapper.Map<DeviceInstallationDocument, DeviceInstallationDto>(deviceInstallation)).ToList();
			}

			return deviceInstallationDtos;
		}

		#endregion

		#region Delete Installations

		public async Task<OperationStatus> DeleteInstallationById(string installationId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var operationStatus = new OperationStatus { IsOperationSuccessful = true };

			// get device installation 
			var deviceInstallation = await GetInstallationByIdAsync(installationId, cancellationToken);
			if (deviceInstallation != null)
			{
				// delete device installation
				operationStatus = await _exceptionDecorator.ExecuteOperationAsync(operationStatus, () => _notificationHubClient.DeleteInstallationAsync(installationId, cancellationToken));
				if (operationStatus.IsOperationSuccessful)
				{
					// delete installation record
					operationStatus = await _deviceInstallationContainer.DeleteItemAsync<DeviceInstallationDocument>(installationId, deviceInstallation.UserId.ToString(), cancellationToken);
				}
			}
			return operationStatus;
		}

		#endregion

	}
}