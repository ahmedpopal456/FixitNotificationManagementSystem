using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Decorators.Exceptions;
using Fixit.Core.Storage.Queue.Mediators;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Mediators.Internal;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fixit.Notification.Management.Lib.UnitTests.Mediators
{
	[TestClass]
	public class NotificationMediatorTests : TestBase
	{
		private INotificationMediator _notificationMediator;

		private readonly string _notificationQueueName = "TestNotificationQueueName";

		private Mock<IQueueClientMediator> _fakeQueueClientMediator;

		[TestInitialize]
		public void TestInitialize()
		{
			_fakeConfiguration = new Mock<IConfiguration>();
			_fakeQueueServiceClientMediator = new Mock<IQueueServiceClientMediator>();
			_fakeQueueClientMediator = new Mock<IQueueClientMediator>();

			_fakeConfiguration.Setup(configuration => configuration["FIXIT-NMS-QUEUE-NAME"])
												.Returns(_notificationQueueName);

			_fakeQueueServiceClientMediator.Setup(queueServiceClientMediator => queueServiceClientMediator.GetQueueClient(_notificationQueueName))
																		 .Returns(_fakeQueueClientMediator.Object);

			_notificationMediator = new NotificationMediator(_mapperConfiguration.CreateMapper(),
																											 _fakeQueueServiceClientMediator.Object,
																											 _fakeConfiguration.Object);
		}

		[TestMethod]
		public async Task EnqueueNotificationAsync_AllRequestsSuccess_ReturnsSuccessful()
		{
			// Arrange
			// Arrange
			var cancellationToken = CancellationToken.None;
			var operationStatus = new OperationStatus { IsOperationSuccessful = true, OperationMessage = "Created" };
			var enqueueNotificationRequestDto = _fakeDtoSeederFactory.CreateSeederFactory(new EnqueueNotificationRequestDto()).First();

			_fakeQueueClientMediator.Setup(queueClientMediator => queueClientMediator.SendMessageAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
															.Returns(Task.FromResult(operationStatus));

			// Act
			var actionResult = await _notificationMediator.EnqueueNotificationAsync(enqueueNotificationRequestDto, cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsTrue(actionResult.IsOperationSuccessful);
			Assert.AreEqual(actionResult.OperationMessage, "Created");
		}
	}
}
