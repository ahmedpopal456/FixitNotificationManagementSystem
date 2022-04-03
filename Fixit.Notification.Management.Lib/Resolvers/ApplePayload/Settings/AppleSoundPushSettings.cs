using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Resolvers.ApplePayload.Settings
{
  public class AppleSoundPushSettings
  {
    [JsonProperty(PropertyName = "alert")]
    public string Alert { get; set; }
  }
}
