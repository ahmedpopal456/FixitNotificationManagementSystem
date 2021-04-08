using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload.Settings
{
	public class AppleSilentPushSettings
	{
		[JsonProperty(PropertyName = "content-available")]
		public int ContentAvailable { get; set; }

		[JsonProperty(PropertyName = "apns-priority")]
		public int ApnsPriority { get; set; }

		[JsonProperty(PropertyName = "sound")]
		public string Sound { get; set; }

		[JsonProperty(PropertyName = "badge")]
		public int Badge { get; set; }
	}
}