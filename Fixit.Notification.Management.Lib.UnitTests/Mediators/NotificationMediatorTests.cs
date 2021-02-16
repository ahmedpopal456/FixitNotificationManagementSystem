using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fixit.Notification.Management.Lib.UnitTests.Mediators
{
	[TestClass]
	public class NotificationMediatorTests : TestBase
	{
		[TestInitialize]
		public void TestInitialize()
		{
		}

		[TestMethod]
		public async Task EnqueueNotificationAsync_Placeholder_ReturnsTrue()
		{
			// Arrange
			bool dummyVar = true;

			// Act

			// Assert
			Assert.IsTrue(dummyVar);
		}
	}
}
