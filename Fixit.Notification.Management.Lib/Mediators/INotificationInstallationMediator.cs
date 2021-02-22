using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Notification.Management.Lib.Models.Notifications.Installations;
using Fixit.Core.DataContracts;
using Microsoft.Azure.NotificationHubs;
using Fixit.Notification.Management.Lib.Models.Notifications;

namespace Fixit.Notification.Management.Lib.Mediators
{
	public interface INotificationInstallationMediator
  {
    /// <summary>
    /// Create of Update an installation for a device and user
    /// </summary>
    /// <param name="deviceInstallationUpsertRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<OperationStatus> UpsertInstallationAsync(DeviceInstallationUpsertRequestDto deviceInstallationUpsertRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// Get an installation by installation id
    /// </summary>
    /// <param name="installationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<DeviceInstallationDto> GetInstallationByIdAsync(string installationId, CancellationToken cancellationToken);

    /// <summary>
    /// Get many installations by filters
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="platformType"></param>
    /// <param name="tags"></param>
    /// <param name="userIds"></param>
    /// <returns></returns>
    public Task<IEnumerable<DeviceInstallationDto>> GetInstallationsAsync(CancellationToken cancellationToken, NotificationPlatform? platformType = null, IEnumerable<NotificationTagDto> tags = null, IEnumerable<Guid> userIds = null);

    /// <summary>
    /// Delete an installation by installation id
    /// </summary>
    /// <param name="installationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<OperationStatus> DeleteInstallationById(string installationId, CancellationToken cancellationToken);
  }
}