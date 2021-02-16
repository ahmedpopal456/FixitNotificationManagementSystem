using System.Threading;
using System.Threading.Tasks;
using Fixit.Core.DataContracts;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;

namespace Fixit.Notification.Management.Lib.Mediators
{
  public interface INotificationMediator
  {
    /// <summary>
    /// Enqueue notification request
    /// </summary>
    /// <param name="enqueueNotificationRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<OperationStatus> EnqueueNotificationAsync(EnqueueNotificationRequestDto enqueueNotificationRequestDto, CancellationToken cancellationToken);
  }
}
