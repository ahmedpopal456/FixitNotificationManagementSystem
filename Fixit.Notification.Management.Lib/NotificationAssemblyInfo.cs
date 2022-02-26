using Fixit.Core.DataContracts.Events.EventGrid.Managers;

namespace Fixit.Notification.Management.Lib
{
  public static class NotificationAssemblyInfo
  {
    public delegate IEventGridTopicServiceClient EventGridTopicServiceClientResolver(string key);

    public static readonly string DataVersion = "1.0";
  }
}
