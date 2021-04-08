using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests
{
	[DataContract]
	public class NotificationQueueRequestDto : NotificationDto
	{
		[DataMember]
		public new object Payload { get; set; }
	}
}
