using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Microsoft.Azure.NotificationHubs;

namespace Fixit.Notification.Management.Lib.Resolvers
{
  public static class NotificationPayloadConverter
  {
    public static string Serialize(NotificationCreateRequestDto notificationCreateRequestDto, NotificationPlatform notificationPlatform)
    {
      return NotificationPayloadResolver.Resolve(notificationCreateRequestDto.Payload,
                                                 notificationCreateRequestDto.Id,
                                                 notificationCreateRequestDto.Title,
                                                 notificationCreateRequestDto.Message,
                                                 notificationCreateRequestDto.Silent,
                                                 notificationPlatform);
    }
  }
}