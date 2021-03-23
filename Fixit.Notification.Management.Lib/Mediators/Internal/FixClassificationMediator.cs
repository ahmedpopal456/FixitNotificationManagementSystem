using AutoMapper;
using Fixit.Core.DataContracts.Users;
using Fixit.Notification.Management.Lib.Builders;
using Fixit.Notification.Management.Lib.Models;
using Fixit.Notification.Management.Lib.Networking.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Notification.Management.Lib.Mediators.Internal
{
    public class FixClassificationMediator : IFixClassificationMediator
    {
        private readonly IMapper _mapper;
        private readonly IFixItHttpClient _fixItHttpClient;

        public FixClassificationMediator(IMapper mapper,
                                      IFixItHttpClient httpClient)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException($"{nameof(FixClassificationMediator)} expects a value for {nameof(httpClient)}... null argument was provided");
            }

            _mapper = mapper ?? throw new ArgumentNullException($"{nameof(FixClassificationMediator)} expects a value for {nameof(mapper)}... null argument was provided");
            _fixItHttpClient = httpClient;
        }

        public async Task<List<UserSummaryDto>> GetMinimalQualitifedCraftmen(FixDocument fixDocument, CancellationToken cancellationToken)
        {
            List<UserDto> users = await _fixItHttpClient.GetUsers("Craftsman", cancellationToken);
            List<UserDocument> craftsmenList = new List<UserDocument>();
            users?.ForEach(users =>
            {
                craftsmenList.Add(_mapper.Map<UserDto, UserDocument>(users));
            });
            List<RatingListDocument> ratingList = await _fixItHttpClient.GetRatings(cancellationToken);

            var builder = new FixClassificationBuilder(craftsmenList);
            builder.ClassifyCraftsmenRating(ratingList);

            // Get Craftsmen List
            var newCraftsmenList = builder.GetCraftsmenScores().Select(craftsman => craftsman.UserDocument).ToList();
            var result = new List<UserSummaryDto>();
            newCraftsmenList.ForEach(craftsmen =>
            {
                result.Add(_mapper.Map<UserDocument, UserSummaryDto>(craftsmen));
            });

            return result;
        }

    }
}
