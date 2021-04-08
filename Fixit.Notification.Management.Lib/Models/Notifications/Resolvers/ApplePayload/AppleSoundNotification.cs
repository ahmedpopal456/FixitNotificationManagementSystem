using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload.Settings;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload
{
	[DataContract]
	public class AppleSoundNotification
	{
		[DataMember, JsonProperty(PropertyName = "aps")]
		public AppleSoundPushSettings ApplePushSettings { get; set; }

		[DataMember, JsonProperty(PropertyName = "action")]
		public string Action { get; set; }

		[DataMember, JsonProperty(PropertyName = "fixitdata")]
		public object FixitData { get; set; }
	}
}
