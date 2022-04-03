using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Data
{
  [DataContract]
  public class ApnsData
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Action { get; set; }

    [DataMember]
    public object SystemPayload { get; set; }
  }
}
