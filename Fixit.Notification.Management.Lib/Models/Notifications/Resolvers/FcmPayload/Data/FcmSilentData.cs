using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Data
{
	[DataContract]
	public class FcmSilentData
	{
		[DataMember, JsonProperty(PropertyName = "message")]
		public string Message { get; set; }


		[DataMember, JsonProperty(PropertyName = "action")]
		public string Action { get; set; }

		[DataMember, JsonProperty(PropertyName = "fixitdata")]
		public object FixitData { get; set; }
	}
}
