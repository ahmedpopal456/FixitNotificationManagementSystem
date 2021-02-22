using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Payloads
{
	[DataContract]
	public class PayloadBaseDto
	{
		[DataMember]
		public Guid Id { get; set; }
	}
}
