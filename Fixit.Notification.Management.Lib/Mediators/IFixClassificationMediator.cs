using Fixit.Core.DataContracts.Users;
using Fixit.Notification.Management.Lib.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fixit.Notification.Management.Lib.Mediators
{
  public interface IFixClassificationMediator
  {
    /// <summary>
    /// Returns a list of qualified craftsmen to be notified
    /// </summary>
    /// <param name="fixDocument">Fix request used to classify craftsmen</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns list of qualified craftsmen</returns>
    Task<List<UserBaseDto>> GetMinimalQualifiedCraftsmen(FixDocument fixDocument, CancellationToken cancellationToken);
  }
}