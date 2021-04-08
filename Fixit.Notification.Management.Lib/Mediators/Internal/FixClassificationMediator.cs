using AutoMapper;
using Fixit.Core.DataContracts.Classifications;
using Fixit.Core.DataContracts.Users;
using Fixit.Notification.Management.Lib.Builders;
using Fixit.Notification.Management.Lib.Models;
using Fixit.Core.DataContracts.Users.Operations.Ratings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.Networking.Local.UMS;
using Fixit.Core.Networking.Local.MDM;
using Microsoft.Extensions.Configuration;

namespace Fixit.Notification.Management.Lib.Mediators.Internal
{
    public class FixClassificationMediator : IFixClassificationMediator
    {
        private readonly IMapper _mapper;
        private readonly IFixUmsHttpClient _fixItHttpUmClient;
        private readonly IFixMdmHttpClient _fixItHttpMdmClient;
        private readonly string _distanceMatrixUri;
        private readonly string _googleApiKey;

        public FixClassificationMediator(IMapper mapper,
                                      IFixUmsHttpClient httpUmClient,
                                      IFixMdmHttpClient httpMdmClient,
                                      IConfiguration configurationProvider)
        {
            _distanceMatrixUri = configurationProvider["FIXIT-GOOGLE-DISTANCEMATRIX-URI"];
            _googleApiKey = configurationProvider["FIXIT-GOOGLE-API-KEY"];
            if (httpUmClient == null)
            {
                throw new ArgumentNullException($"{nameof(FixClassificationMediator)} expects a value for {nameof(httpUmClient)}... null argument was provided");
            }
            if (httpMdmClient == null)
            {
                throw new ArgumentNullException($"{nameof(FixClassificationMediator)} expects a value for {nameof(httpMdmClient)}... null argument was provided");
            }

            _mapper = mapper ?? throw new ArgumentNullException($"{nameof(FixClassificationMediator)} expects a value for {nameof(mapper)}... null argument was provided");
            _fixItHttpUmClient = httpUmClient;
            _fixItHttpMdmClient = httpMdmClient;
        }

        public FixClassificationMediator(IMapper mapper,
                              IFixUmsHttpClient httpUmClient,
                              IFixMdmHttpClient httpMdmClient,
                              string distanceMatrixUri,
                              string googleApiKey)
        {
            if (distanceMatrixUri == null)
            {
                throw new ArgumentNullException($"{nameof(FixClassificationMediator)} expects a value for {nameof(distanceMatrixUri)}... null argument was provided");
            }
            if (googleApiKey == null)
            {
                throw new ArgumentNullException($"{nameof(FixClassificationMediator)} expects a value for {nameof(googleApiKey)}... null argument was provided");
            }
            if (httpUmClient == null)
            {
                throw new ArgumentNullException($"{nameof(FixClassificationMediator)} expects a value for {nameof(httpUmClient)}... null argument was provided");
            }
            if (httpMdmClient == null)
            {
                throw new ArgumentNullException($"{nameof(FixClassificationMediator)} expects a value for {nameof(httpMdmClient)}... null argument was provided");
            }

            _mapper = mapper ?? throw new ArgumentNullException($"{nameof(FixClassificationMediator)} expects a value for {nameof(mapper)}... null argument was provided");
            _fixItHttpUmClient = httpUmClient;
            _fixItHttpMdmClient = httpMdmClient;
            _distanceMatrixUri = distanceMatrixUri;
            _googleApiKey = googleApiKey;

        }

        public async Task<List<UserSummaryDto>> GetMinimalQualitifedCraftmen(FixDocument fixDocument, CancellationToken cancellationToken)
        {
            List<UserDocument> craftsmenList = await GetCraftsmenList(cancellationToken);
            List<RatingListDocument> ratingList = await GetRatingList(cancellationToken);
            IEnumerable<WorkCategoryDto> skillList = await GetQualifiedSkillList(fixDocument, cancellationToken);

            var builder = new FixClassificationBuilder(fixDocument, craftsmenList);
            builder.ClassifyCraftsmenLocation(_distanceMatrixUri, _googleApiKey)
                    .ClassifyCraftsmenSkill(skillList.FirstOrDefault()?.Skills)
                    .ClassifyCraftsmenAvailability()
                    .ClassifyCraftsmenRating(ratingList)
                    .GetQualifiedCraftsmenByAverage();

            List<UserSummaryDto> result = GetQualifiedCraftsmenList(builder);
            return result;
        }

        private List<UserSummaryDto> GetQualifiedCraftsmenList(FixClassificationBuilder builder)
        {
            var newCraftsmenList = builder.GetCraftsmenScores().Select(craftsman => craftsman.UserDocument).ToList();
            var result = new List<UserSummaryDto>();
            newCraftsmenList.ForEach(craftsmen =>
            {
                result.Add(_mapper.Map<UserDocument, UserSummaryDto>(craftsmen));
            });
            return result;
        }

        private async Task<IEnumerable<WorkCategoryDto>> GetQualifiedSkillList(FixDocument fixDocument, CancellationToken cancellationToken)
        {
            // Get qualified Skills for fix request
            var fixDetails = fixDocument.Details.FirstOrDefault();
            var skillList = await _fixItHttpMdmClient.GetWorkCategoriesAsync(cancellationToken, fixDetails.Category);
            return skillList;
        }

        private async Task<List<RatingListDocument>> GetRatingList(CancellationToken cancellationToken)
        {
            // Get Craftsman user ratings
            var ratings = await _fixItHttpUmClient.GetRatings(cancellationToken);
            List<RatingListDocument> ratingList = new List<RatingListDocument>();
            ratings?.ForEach(rating => { ratingList.Add(_mapper.Map<RatingListResponseDto, RatingListDocument>(rating)); });
            return ratingList;
        }

        private async Task<List<UserDocument>> GetCraftsmenList(CancellationToken cancellationToken)
        {
            // Get Craftsman users
            List<UserDto> users = await _fixItHttpUmClient.GetUsers("Craftsman", cancellationToken);
            List<UserDocument> craftsmenList = new List<UserDocument>();
            users?.ForEach(user => { craftsmenList.Add(_mapper.Map<UserDto, UserDocument>(user)); });
            return craftsmenList;
        }
    }
}
