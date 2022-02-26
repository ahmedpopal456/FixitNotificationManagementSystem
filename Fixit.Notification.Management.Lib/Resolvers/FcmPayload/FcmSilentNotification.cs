using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Resolvers.FcmPayload.Data;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Resolvers.FcmPayload
{
  [DataContract]
  public class FcmSilentNotification
  {
    [DataMember, JsonProperty(PropertyName = "data")]
    public FcmSilentData Data { get; set; }
  }
}
