using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.Database.DataContracts.Documents;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Decorators.Exceptions;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Mediators.Internal;
using Fixit.Notification.Management.Lib.Models.Notifications.Installations;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fixit.Notification.Management.Lib.UnitTests.Mediators
{
	[TestClass]
	public class NotificationInstallationMediatorTests : TestBase
	{
		private INotificationInstallationMediator _notificationInstallationMediator;
		private ILogger<NotificationInstallationMediator> _fakeLogger;

		private readonly string _userDatabaseName = "TestDatabaseName";
		private readonly string _userDatabaseContainerName = "TestDeviceInstallationsContainerName";

		[TestInitialize]
		public void TestInitialize()
		{
			_fakeConfiguration = new Mock<IConfiguration>();
			_fakeDatabaseMediator = new Mock<IDatabaseMediator>();
			_fakeDatabaseTableMediator = new Mock<IDatabaseTableMediator>();
			_fakeDatabaseTableEntityMediator = new Mock<IDatabaseTableEntityMediator>();
			_fakeNotificationHubClient = new Mock<INotificationHubClient>();
			_fakeLogger = new Mock<ILogger<NotificationInstallationMediator>>().Object;
			_fakeOperationStatusExceptionDecorator = new Mock<IExceptionDecorator<OperationStatus>>();

			_fakeConfiguration.Setup(configuration => configuration["FIXIT-NMS-DB-NAME"])
												.Returns(_userDatabaseName);

			_fakeConfiguration.Setup(configuration => configuration["FIXIT-NMS-DB-INSTALLATIONS"])
												.Returns(_userDatabaseContainerName);

			_fakeDatabaseMediator.Setup(databaseMediator => databaseMediator.GetDatabase(_userDatabaseName))
													 .Returns(_fakeDatabaseTableMediator.Object);

			_fakeDatabaseTableMediator.Setup(database => database.GetContainer(_userDatabaseContainerName))
																.Returns(_fakeDatabaseTableEntityMediator.Object);

			_notificationInstallationMediator = new NotificationInstallationMediator(_fakeDatabaseMediator.Object,
																																							 _mapperConfiguration.CreateMapper(),
																																							 _fakeNotificationHubClient.Object,
																																							 _fakeConfiguration.Object,
																																							 _fakeLogger,
																																							 _fakeOperationStatusExceptionDecorator.Object);
		}

		#region UpsertInstallationAsync
		[TestMethod]
		public async Task UpsertInstallationAsync_AllRequestsSuccess_ReturnsSuccessful()
		{
			// Arrange
			var cancellationToken = CancellationToken.None;
			var initialOperationStatus = new OperationStatus { IsOperationSuccessful = true };
			var deviceInstallationUpsertRequestDto = _fakeDtoSeederFactory.CreateSeederFactory(new DeviceInstallationUpsertRequestDto()).First();
			var updatedOperationStatus = new OperationStatus { IsOperationSuccessful = true, OperationMessage = HttpStatusCode.OK.ToString() };

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.CreateOrUpdateInstallationAsync(It.IsAny<Installation>(), It.IsAny<CancellationToken>()))
																.Returns(Task.CompletedTask);

			_fakeOperationStatusExceptionDecorator.Setup(exceptionDecorator => exceptionDecorator.ExecuteOperationAsync(It.IsAny<OperationStatus>(), It.IsAny<Func<Task>>()))
																						.Returns(Task.FromResult(initialOperationStatus));

			_fakeDatabaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.UpdateItemAsync(It.IsAny<DeviceInstallationDocument>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
																			.Returns(Task.FromResult(updatedOperationStatus));

			// Act
			var actionResult = await _notificationInstallationMediator.UpsertInstallationAsync(deviceInstallationUpsertRequestDto, cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsTrue(actionResult.IsOperationSuccessful);
			Assert.AreEqual(actionResult.OperationMessage, HttpStatusCode.OK.ToString());
		}

		[TestMethod]
		public async Task UpsertInstallationAsync_NotificationHubClientRejection_ReturnsUnsuccessful()
		{
			// Arrange
			var cancellationToken = CancellationToken.None;
			var deviceInstallationUpsertRequestDto = _fakeDtoSeederFactory.CreateSeederFactory(new DeviceInstallationUpsertRequestDto()).First();
			var initialOperationStatus = new OperationStatus
			{
				IsOperationSuccessful = false,
				OperationMessage = HttpStatusCode.BadRequest.ToString(),
				OperationException = new Exception()
			};

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.CreateOrUpdateInstallationAsync(It.IsAny<Installation>(), It.IsAny<CancellationToken>()))
																.Returns(Task.CompletedTask);

			_fakeOperationStatusExceptionDecorator.Setup(exceptionDecorator => exceptionDecorator.ExecuteOperationAsync(It.IsAny<OperationStatus>(), It.IsAny<Func<Task>>()))
																						.Returns(Task.FromResult(initialOperationStatus));

			// Act
			var actionResult = await _notificationInstallationMediator.UpsertInstallationAsync(deviceInstallationUpsertRequestDto, cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsFalse(actionResult.IsOperationSuccessful);
			Assert.IsNotNull(actionResult.OperationMessage);
			Assert.IsNotNull(actionResult.OperationException);
		}
		#endregion

		#region GetInstallationByIdAsync
		[TestMethod]
		[DataRow("445e50d1-b2e7-4c25-a628-c610aed7a357", DisplayName = "Known_DeviceId")]
		public async Task GetInstallationByIdAsync_AllRequestsSuccess_Returnssuccessful(string installationId)
		{
			// Arrange
			var cancellationToken = CancellationToken.None;
			var deviceId = "device-123";
			var userId = "445e50d1-b2e7-4c25-a628-c610aed7a357";
			var deviceInstallation = _fakeDtoSeederFactory.CreateSeederFactory(new DeviceInstallationDto());
			var installation = new Installation { InstallationId = deviceId, Tags = new List<string> { $"UserPrefix:{userId}" } };

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.GetInstallationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
																.Returns(Task.FromResult(installation));

			// Act
			var actionResult = await _notificationInstallationMediator.GetInstallationByIdAsync(installationId, cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.AreEqual(actionResult.InstallationId, deviceId);
			Assert.AreEqual(actionResult.UserId, Guid.Parse(userId));
		}

		[TestMethod]
		[DataRow("445e50d1-b2e7-4c25-a628-c610aed7a357", DisplayName = "Known_DeviceId")]
		public async Task GetInstallationByIdAsync_NoUserIdTag_ReturnsSuccessful(string installationId)
		{
			// Arrange
			var cancellationToken = CancellationToken.None;
			var deviceId = "device-123";
			var deviceInstallation = _fakeDtoSeederFactory.CreateSeederFactory(new DeviceInstallationDto());
			var installation = new Installation { InstallationId = deviceId };

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.GetInstallationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
																.Returns(Task.FromResult(installation));

			// Act
			var actionResult = await _notificationInstallationMediator.GetInstallationByIdAsync(installationId, cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.AreEqual(actionResult.InstallationId, deviceId);
			Assert.AreEqual(actionResult.UserId, Guid.Empty);
		}

		[TestMethod]
		[DataRow("445e50d1-b2e7-4c25-a628-c610aed7a357", DisplayName = "Unknown_DeviceId")]
		public async Task GetInstallationByIdAsync_NotificationHubClientRejection_ReturnsNoResult(string installationId)
		{
			// Arrange
			var cancellationToken = CancellationToken.None;

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.GetInstallationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
																.Returns(Task.FromResult(default(Installation)));

			// Act
			var actionResult = await _notificationInstallationMediator.GetInstallationByIdAsync(installationId, cancellationToken);

			// Assert
			Assert.IsNull(actionResult);
		}
		#endregion

		#region GetInstallationsAsync
		[TestMethod]
		public async Task GetInstallationsAsync_FilterByUserIds_ReturnsResults()
		{
			// Arrange
			var cancellationToken = CancellationToken.None;
			var userIdList = new List<Guid> { Guid.Parse("445e50d1-b2e7-4c25-a628-c610aed7a357") };
			var deviceInstallationDocuments = _fakeDtoSeederFactory.CreateSeederFactory(new DeviceInstallationDocument());
			var documentCollectionDto = new DocumentCollectionDto<DeviceInstallationDocument>
			{
				Results = deviceInstallationDocuments,
				IsOperationSuccessful = true
			};

			_fakeDatabaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<DeviceInstallationDocument, bool>>>(), null))
																			.Returns(Task.FromResult<(DocumentCollectionDto<DeviceInstallationDocument>, string)>((documentCollectionDto, null)));

			// Act
			var actionResult = await _notificationInstallationMediator.GetInstallationsAsync(cancellationToken, userIds: userIdList);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsTrue(actionResult.Any());
		}

		[TestMethod]
		public async Task GetInstallationsAsync_NoMatch_ReturnsEmptyList()
		{
			// Arrange
			var cancellationToken = CancellationToken.None;
			var userIdList = new List<Guid> { Guid.Parse("445e50d1-b2e7-4c25-a628-c610aed7a357") };
			var deviceInstallationDocuments = _fakeDtoSeederFactory.CreateSeederFactory(new DeviceInstallationDocument());
			var documentCollectionDto = new DocumentCollectionDto<DeviceInstallationDocument>
			{
				Results = new List<DeviceInstallationDocument>(),
				IsOperationSuccessful = true
			};

			_fakeDatabaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<DeviceInstallationDocument, bool>>>(), null))
																			.Returns(Task.FromResult<(DocumentCollectionDto<DeviceInstallationDocument>, string)>((documentCollectionDto, null)));

			// Act
			var actionResult = await _notificationInstallationMediator.GetInstallationsAsync(cancellationToken, userIds: userIdList);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsFalse(actionResult.Any());
		}

		[TestMethod]
		public async Task GetInstallationsAsync_NotSuccessful_ReturnsEmptyList()
		{
			// Arrange
			var cancellationToken = CancellationToken.None;
			var userIdList = new List<Guid> { Guid.Parse("445e50d1-b2e7-4c25-a628-c610aed7a357") };
			var deviceInstallationDocuments = _fakeDtoSeederFactory.CreateSeederFactory(new DeviceInstallationDocument());
			var documentCollectionDto = new DocumentCollectionDto<DeviceInstallationDocument>
			{
				Results = new List<DeviceInstallationDocument>(),
				IsOperationSuccessful = false
			};

			_fakeDatabaseTableEntityMediator.Setup(databaseTableEntityMediator => databaseTableEntityMediator.GetItemQueryableAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<DeviceInstallationDocument, bool>>>(), null))
																			.Returns(Task.FromResult<(DocumentCollectionDto<DeviceInstallationDocument>, string)>((documentCollectionDto, null)));

			// Act
			var actionResult = await _notificationInstallationMediator.GetInstallationsAsync(cancellationToken, userIds: userIdList);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsFalse(actionResult.Any());
		}
		#endregion

		#region DeleteInstallationById
		[TestMethod]
		[DataRow("445e50d1-b2e7-4c25-a628-c610aed7a357", DisplayName = "Known_DeviceId")]
		public async Task DeleteInstallationById_AllRequestsSuccess_ReturnsSuccessfult(string installationId)
		{
			// Arrange
			var cancellationToken = CancellationToken.None;
			var deviceId = "device-123";
			var userId = "445e50d1-b2e7-4c25-a628-c610aed7a357";
			var deviceInstallation = _fakeDtoSeederFactory.CreateSeederFactory(new DeviceInstallationDto());
			var installation = new Installation { InstallationId = deviceId, Tags = new List<string> { $"userId:{userId}" } };
			var initialOperationStatus = new OperationStatus { IsOperationSuccessful = true };
			var updatedOperationStatus = new OperationStatus { IsOperationSuccessful = true, OperationMessage = HttpStatusCode.OK.ToString() };

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.GetInstallationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
																.Returns(Task.FromResult(installation));

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.DeleteInstallationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
																.Returns(Task.CompletedTask);

			_fakeOperationStatusExceptionDecorator.Setup(exceptionDecorator => exceptionDecorator.ExecuteOperationAsync(It.IsAny<OperationStatus>(), It.IsAny<Func<Task>>()))
																						.Returns(Task.FromResult(initialOperationStatus));

			_fakeDatabaseTableEntityMediator.Setup(database => database.DeleteItemAsync<DeviceInstallationDocument>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
																			.Returns(Task.FromResult(updatedOperationStatus));

			// Act
			var actionResult = await _notificationInstallationMediator.DeleteInstallationById(installationId, cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsTrue(actionResult.IsOperationSuccessful);
			Assert.AreEqual(actionResult.OperationMessage, HttpStatusCode.OK.ToString());
		}

		[TestMethod]
		[DataRow("445e50d1-b2e7-4c25-a628-c610aed7a357", DisplayName = "Known_DeviceId")]
		public async Task DeleteInstallationById_NotificationHubRejection_ReturnsUnsuccessfult(string installationId)
		{
			// Arrange
			var cancellationToken = CancellationToken.None;
			var deviceId = "device-123";
			var userId = "445e50d1-b2e7-4c25-a628-c610aed7a357";
			var deviceInstallation = _fakeDtoSeederFactory.CreateSeederFactory(new DeviceInstallationDto());
			var installation = new Installation { InstallationId = deviceId, Tags = new List<string> { $"userId:{userId}" } };
			var initialOperationStatus = new OperationStatus { IsOperationSuccessful = false, OperationException = new Exception("Exception Message") };

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.GetInstallationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
																.Returns(Task.FromResult(installation));

			_fakeNotificationHubClient.Setup(notificationHubClient => notificationHubClient.DeleteInstallationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
																.Returns(Task.CompletedTask);

			_fakeOperationStatusExceptionDecorator.Setup(exceptionDecorator => exceptionDecorator.ExecuteOperationAsync(It.IsAny<OperationStatus>(), It.IsAny<Func<Task>>()))
																						.Returns(Task.FromResult(initialOperationStatus));

			// Act
			var actionResult = await _notificationInstallationMediator.DeleteInstallationById(installationId, cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsFalse(actionResult.IsOperationSuccessful);
			Assert.IsNotNull(actionResult.OperationException);
		}
		#endregion
	}
}
