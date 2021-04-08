using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload.Settings
{
	public class AppleSoundPushSettings
	{
		[JsonProperty(PropertyName = "alert")]
		public object Alert { get; set; }
	}
}
