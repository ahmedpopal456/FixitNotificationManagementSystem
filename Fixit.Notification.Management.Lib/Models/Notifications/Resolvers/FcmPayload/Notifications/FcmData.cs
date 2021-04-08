using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Notifications
{
	[DataContract]
	public class FcmData
	{
		[DataMember, JsonProperty(PropertyName = "action")]
		public string Action { get; set; }

		[DataMember, JsonProperty(PropertyName = "fixitdata")]
		public object FixitData { get; set; }
	}
}
