using System;
using System.Runtime.Serialization;
using Fixit.Core.DataContracts.Notifications.Payloads;
using Fixit.Core.DataContracts.Users;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests
{
  [DataContract]
  public class NotificationCreateRequestDto : IDtoValidator
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public NotificationPayloadDto Payload { get; set; }

    [DataMember]
    public UserBaseDto RecipientUser { get; set; }

    [DataMember]
    public bool Silent { get; set; }

    #region IDtoValidator

    public bool Validate()
    {
      bool isValid = ((Title != null) 
                     && (Payload != null || !string.IsNullOrWhiteSpace(Message))
                     && (RecipientUser != null && RecipientUser.Id != Guid.Empty));

      return isValid;
    }

    #endregion
  }
}
