using Fixit.Core.DataContracts;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Operations.Responses
{
  [DataContract]
  public class NotificationsCreateResponse: OperationStatus
  {
    public NotificationsCreateResponse(OperationStatus operationStatus)
    {
      IsOperationSuccessful = operationStatus.IsOperationSuccessful;
      OperationException = operationStatus.OperationException;
    }

    [DataMember]
    public NotificationDocument Notification {get; set;}
  }
}
