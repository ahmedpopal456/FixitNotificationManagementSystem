using Fixit.Core.DataContracts.Users.Skills;
using Fixit.Core.Networking.Local.MDM;
using Fixit.Core.Networking.Local.UMS;
using Fixit.Notification.Management.Lib.Builders;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Mediators.Internal;
using Fixit.Notification.Management.Lib.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Notification.Management.Lib.UnitTests.Mediators
{
    [TestClass]
    public class FixClassificationTests : TestBase
    {
		private FixClassificationMediator _fixAndCraftsmenMatchMediator;
		private CancellationToken _cancellationToken;

        private Mock<IFixClassificationMediator> _fakeFixClassificationMediator;
        private Mock<IFixUmsHttpClient> _fakeHttpUmClient;
        private Mock<IFixMdmHttpClient> _fakeHttpMdMClient;
        private readonly string _distanceMatrixUrl = "DistanceMatrixUrl";
        private readonly string _googleApiKey = "GoogleApiKey";

		[TestInitialize]
		public void TestInitialize()
		{
			_fakeConfiguration = new Mock<IConfiguration>();
			_fakeFixClassificationMediator = new Mock<IFixClassificationMediator>();
			_fakeHttpUmClient = new Mock<IFixUmsHttpClient>();
			_fakeHttpMdMClient = new Mock<IFixMdmHttpClient>();

			_cancellationToken = CancellationToken.None;
			_fixAndCraftsmenMatchMediator = new FixClassificationMediator(_mapperConfiguration.CreateMapper(), _fakeHttpUmClient.Object, _fakeHttpMdMClient.Object, _distanceMatrixUrl, _googleApiKey);
		}

        #region FixClassificationMediator

		public async Task GetMinimalQualitifedCraftmen_ReturnsSuccess()
        {
			// Arrange
			var cancellationToken = CancellationToken.None;
			var fixDocument = new FixDocument().SeedFakeDtos().First();

			// Act
			var actionResult = await _fixAndCraftsmenMatchMediator.GetMinimalQualitifedCraftmen(fixDocument, cancellationToken);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.AreEqual(0, actionResult.Count);
		}

        #endregion

        #region FixClassificationBuilder
        [TestMethod]
		public void ClassifyCraftsmenSkill_ReturnsSuccess()
        {
            // Arrange
            var craftsman = new UserDocument().SeedFakeDtos().First();
            var fixDocument = new FixDocument().SeedFakeDtos().First();
            var craftsmenList = new List<UserDocument>() { craftsman };
            var skill = new SkillDto() { Id = new Guid("ce59667a-5df5-4fa8-907f-80637ece426d"), Name = "Small Concrete Works" };
            var skillList = new List<SkillDto>() { skill };
            // Act
            var actionResult = new FixClassificationBuilder(fixDocument, craftsmenList);
            actionResult.ClassifyCraftsmenSkill(skillList);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod]
		[ExpectedException(typeof(UriFormatException), "Invalid URI: The format of the URI could not be determined.")]
		public void ClassifyCraftsmenLocation_ReturnsInvalidUriException()
		{
			// Arrange
			var craftsman = new UserDocument().SeedFakeDtos().First();
			var fixDocument = new FixDocument().SeedFakeDtos().First();
			var craftsmenList = new List<UserDocument>() { craftsman };

			// Act
			var actionResult = new FixClassificationBuilder(fixDocument, craftsmenList);
			actionResult.ClassifyCraftsmenLocation(_distanceMatrixUrl, _googleApiKey);

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.AreEqual(1, actionResult.GetCraftsmenScores().Count);
		}
		 
		[TestMethod]
		public void ClassifyCraftsmenAvailability_ReturnsSuccess()
		{
			// Arrange
			var craftsman = new UserDocument().SeedFakeDtos().First();
			var fixDocument = new FixDocument().SeedFakeDtos().First();
			var craftsmenList = new List<UserDocument>() { craftsman };

			// Act
			var actionResult = new FixClassificationBuilder(fixDocument, craftsmenList);
			actionResult.ClassifyCraftsmenAvailability();

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.AreEqual(1, actionResult.GetCraftsmenScores().Count);
		}


		[TestMethod]
		public void GetQualifiedCraftsmenByAverage_ReturnsSuccess()
		{
			// Arrange
			var craftsman = new UserDocument().SeedFakeDtos().First();
			var fixDocument = new FixDocument().SeedFakeDtos().First();
			var craftsmenList = new List<UserDocument>() { craftsman };

			// Act
			var actionResult = new FixClassificationBuilder(fixDocument, craftsmenList);
			actionResult.GetQualifiedCraftsmenByAverage();

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.AreEqual(1, actionResult.GetCraftsmenScores().Count);
		}

		[TestMethod]
		public void GetCrafstmenTotalScore_ReturnsSuccess()
		{
			// Arrange
			var craftsman = new UserDocument().SeedFakeDtos().First();
			var fixDocument = new FixDocument().SeedFakeDtos().First();
			var craftsmenList = new List<UserDocument>() { craftsman };

			// Act
			var actionResult = new FixClassificationBuilder(fixDocument, craftsmenList);
			actionResult.GetCrafstmenTotalScore();

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.AreEqual(1, actionResult.GetCraftsmenScores().Count);
		}

		[TestMethod]
		public void GetCraftsmenScores_ReturnsSuccess()
		{
			// Arrange
			var craftsman = new UserDocument().SeedFakeDtos().First();
			var fixDocument = new FixDocument().SeedFakeDtos().First();
			var craftsmenList = new List<UserDocument>() { craftsman };

			// Act
			var builder = new FixClassificationBuilder(fixDocument, craftsmenList);
			var actionResult = builder.GetCraftsmenScores();

			// Assert
			Assert.IsNotNull(actionResult);
			Assert.AreEqual(1, actionResult.Count);
		}

        #endregion
    }
}
