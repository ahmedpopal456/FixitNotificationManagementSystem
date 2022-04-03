using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Resolvers.FcmPayload.Data
{
  [DataContract]
  public class FcmSilentData
  {
    [DataMember, JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [DataMember, JsonProperty(PropertyName = "message")]
    public string Message { get; set; }

    [DataMember, JsonProperty(PropertyName = "action")]
    public string Action { get; set; }

    [DataMember, JsonProperty(PropertyName = "systemPayload")]
    public object SystemPayload { get; set; }
  }
}
