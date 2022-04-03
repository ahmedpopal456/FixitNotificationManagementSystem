using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Fixit.Notification.Management.Triggers.Functions.Utils
{
  public static class NotificationFunctionUtils
  {
    public static async Task NotifyAndLogError(Func<Task<NotificationOutcome>> notify, NotificationPlatform notificationPlatform, ILogger logger)
    {
      try
      {
        NotificationOutcome notificationOutcome = await notify();
        if (notificationOutcome is null || ((notificationOutcome.State == NotificationOutcomeState.Abandoned) || (notificationOutcome.State == NotificationOutcomeState.Unknown)))
        {
          logger.LogError($"Failed to notify devices with platform {notificationPlatform}...");
        }
      }
      catch(Exception exception)
      {
        logger.LogError($"Unexpected error happened when trying to notify devices with platform {notificationPlatform}...", exception);
      }
    }
  }
}
