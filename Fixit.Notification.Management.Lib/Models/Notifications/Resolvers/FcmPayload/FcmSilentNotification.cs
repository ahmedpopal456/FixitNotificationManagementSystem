using System.Runtime.Serialization;
using Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Data;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload
{
	[DataContract]
	public class FcmSilentNotification
	{
		[DataMember, JsonProperty(PropertyName = "data")]
		public FcmSilentData Data { get; set; }
	}
}
