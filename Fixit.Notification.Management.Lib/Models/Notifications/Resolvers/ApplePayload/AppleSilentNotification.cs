using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload.Settings;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.ApplePayload
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

		// TODO: test in apple device
		[DataMember, JsonProperty(PropertyName = "fixitdata")]
		public object FixitData { get; set; }
	}
}
