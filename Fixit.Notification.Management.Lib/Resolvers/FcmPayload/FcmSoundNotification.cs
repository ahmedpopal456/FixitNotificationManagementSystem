using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Resolvers.FcmPayload.Data;
using Fixit.Notification.Management.Lib.Resolvers.FcmPayload.Notifications;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Resolvers.FcmPayload
{
  [DataContract]
  public class FcmSoundNotification
  {
    [DataMember, JsonProperty(PropertyName = "notification")]
    public FcmNotification Notification { get; set; }

    [DataMember, JsonProperty(PropertyName = "action")]
    public string Action { get; set; }

    [DataMember, JsonProperty(PropertyName = "data")]
    public FcmData Data { get; set; }
  }
}
