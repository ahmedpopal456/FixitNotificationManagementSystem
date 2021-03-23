using Fixit.Core.DataContracts.Users;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.Notification.Management.Lib.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Notification.Management.Lib.Networking.Local
{
    public interface IFixItHttpClient
    {
        Task<List<RatingListDocument>> GetRatings(CancellationToken cancellationToken);

        Task<List<UserDto>> GetUsers(string entityId, CancellationToken cancellationToken);

        Task<UserProfileDto> GetUser(Guid userId, CancellationToken cancellationToken);

    }
}
