using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Resolvers.FcmPayload.Notifications
{
	[DataContract]
	public class FcmNotification
	{
		[DataMember, JsonProperty(PropertyName = "title")]
		public string Title { get; set; }

		[DataMember, JsonProperty(PropertyName = "body")]
		public object Body { get; set; }
	}
}
