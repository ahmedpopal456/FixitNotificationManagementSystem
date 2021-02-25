using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Models.Notifications
{
	/// <summary>
	/// A Notification Tag
	/// </summary>
	[DataContract]
	public class NotificationTagDto
	{
		[DataMember]
		public string Key { get; set; }

		[DataMember]
		public string Value { get; set; }
	}
}
