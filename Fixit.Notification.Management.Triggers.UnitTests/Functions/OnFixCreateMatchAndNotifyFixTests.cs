using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts;
using Fixit.Core.Networking.Local.MDM;
using Fixit.Core.Networking.Local.UMS;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Mediators.Internal;
using Fixit.Notification.Management.Lib.Models;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Notification.Management.Triggers.Functions;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fixit.Notification.Management.Triggers.UnitTests.Functions
{
	[TestClass]
	public class OnFixCreateMatchAndNotifyFixTests : TestBase
	{
		private OnFixCreateMatchAndNotifyFix _onFixCreateMatchAndNotifyFix;
		private FixClassificationMediator _fixAndCraftsmenMatchMediator;
		private Mock<ILoggerFactory> _fakeLoggerFactory;
		private Mock<ILogger<OnFixCreateMatchAndNotifyFix>> _fakeLogger;
		private CancellationToken _cancellationToken;
		private Mock<INotificationMediator> _fakeNotificationMediator;

		private Mock<IFixClassificationMediator> _fakeFixClassificationMediator;
		private Mock<IFixUmsHttpClient> _fakeHttpUmClient;
		private Mock<IFixMdmHttpClient> _fakeHttpMdMClient;
		private readonly string _distanceMatrixUrl = "DistanceMatrixUrl";
		private readonly string _googleApiKey = "GoogleApiKey";

		[TestInitialize]
		public void TestInitialize()
		{
			_fakeConfiguration = new Mock<IConfiguration>();
			_fakeLoggerFactory = new Mock<ILoggerFactory>();
			_fakeLogger = new Mock<ILogger<OnFixCreateMatchAndNotifyFix>>();
			_fakeNotificationMediator = new Mock<INotificationMediator>();
			_fakeFixClassificationMediator = new Mock<IFixClassificationMediator>();
			_fakeHttpUmClient = new Mock<IFixUmsHttpClient>();
			_fakeHttpMdMClient = new Mock<IFixMdmHttpClient>();

			_fakeLoggerFactory.Setup(fakeLoggerFactory => fakeLoggerFactory.CreateLogger(It.IsAny<string>()))
												.Returns(_fakeLogger.Object);

			_cancellationToken = CancellationToken.None;
			_onFixCreateMatchAndNotifyFix = new OnFixCreateMatchAndNotifyFix(_fakeConfiguration.Object, _fakeLoggerFactory.Object, _fakeNotificationMediator.Object, _fakeFixClassificationMediator.Object);
			_fixAndCraftsmenMatchMediator = new FixClassificationMediator(_mapperConfiguration.CreateMapper(), _fakeHttpUmClient.Object, _fakeHttpMdMClient.Object, _distanceMatrixUrl, _googleApiKey);
		}

		#region OnFixCreateMatchAndNotifyFix
		[TestMethod]
		public async Task OnFixCreateMatchAndNotifyFix_NewDocuments_ReturnsCompletedTask()
		{
			// Arrange
			var rawFixDocuments = new List<Document> { new Document() };
			var operationStatus = new OperationStatus { IsOperationSuccessful = true };
			_fakeNotificationMediator.Setup(notificationMediator => notificationMediator.EnqueueNotificationAsync(It.IsAny<EnqueueNotificationRequestDto>(), It.IsAny<CancellationToken>()))
															 .ReturnsAsync(operationStatus);

			// Act
			var actionResult = await _onFixCreateMatchAndNotifyFix.MatchAndNotifyFix(rawFixDocuments, _cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.IsTrue(Convert.ToBoolean(actionResult));
		}

		[TestMethod]
		public async Task OnFixCreateMatchAndNotifyFix_EnqueueFails_ReturnsNotCompletedTask()
		{
			// Arrange
			var rawFixDocuments = new List<Document> { new Document() };
			var operationStatus = new OperationStatus { IsOperationSuccessful = false, OperationException = new Exception("failed") };
			_fakeNotificationMediator.Setup(notificationMediator => notificationMediator.EnqueueNotificationAsync(It.IsAny<EnqueueNotificationRequestDto>(), It.IsAny<CancellationToken>()))
															 .ReturnsAsync(operationStatus);

			// Act and Assert
			await Assert.ThrowsExceptionAsync<AggregateException>(async () => await _onFixCreateMatchAndNotifyFix.MatchAndNotifyFix(rawFixDocuments, _cancellationToken));
		}
		#endregion

	}
}
