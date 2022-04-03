using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Resolvers.FcmPayload.Data
{
  [DataContract]
  public class FcmData
  {
    [DataMember, JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [DataMember, JsonProperty(PropertyName = "action")]
    public string Action { get; set; }

    [DataMember, JsonProperty(PropertyName = "systemPayload")]
    public object SystemPayload { get; set; }
  }
}
