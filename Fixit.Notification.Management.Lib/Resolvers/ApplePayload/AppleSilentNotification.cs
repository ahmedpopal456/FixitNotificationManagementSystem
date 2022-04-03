using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Data;
using Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Settings;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Resolvers.ApplePayload
{
  [DataContract]
  public class AppleSilentNotification
  {
    [DataMember, JsonProperty(PropertyName = "aps")]
    public AppleSilentPushSettings ApplePushSettings { get; set; }

    [DataMember, JsonProperty(PropertyName = "message")]
    public string Message { get; set; }

    [DataMember, JsonProperty(PropertyName = "action")]
    public string Action { get; set; }

    [DataMember, JsonProperty(PropertyName = "data")]
    public ApnsData Data { get; set; }
  }
}
