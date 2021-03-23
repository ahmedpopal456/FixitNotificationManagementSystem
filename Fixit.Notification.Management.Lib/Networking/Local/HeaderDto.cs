using System.Runtime.Serialization;

namespace Fixit.Notification.Management.Lib.Networking.Local
{
	[DataContract]
	public class HeaderDto
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Value { get; set; }
	}
}
