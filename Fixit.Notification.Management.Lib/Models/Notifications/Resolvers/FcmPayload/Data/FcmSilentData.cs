using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Data
{
	[DataContract]
	public class FcmSilentData
	{
		[DataMember, JsonProperty(PropertyName = "message")]
		public object Message { get; set; }


		[DataMember, JsonProperty(PropertyName = "action")]
		public string Action { get; set; }
	}
}
