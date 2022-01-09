using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.DataContracts.Notifications.Payloads;
using Fixit.Core.DataContracts.Seeders;
using Fixit.Core.DataContracts.Users;
using Fixit.Notification.Management.Lib.Models.Notifications.Enums;

namespace Fixit.Notification.Management.Lib.Models.Notifications
{
	[DataContract]
	public class NotificationDto : IFakeSeederAdapter<NotificationDto>
	{
		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public object Payload { get; set; }

		[DataMember]
		public NotificationTypes Action { get; set; }

		[DataMember]
		public IList<NotificationTagDto> Tags { get; set; }

		[DataMember]
		public IEnumerable<UserSummaryDto> Recipients { get; set; }

		[DataMember]
		public bool Silent { get; set; }

		[DataMember]
		public long CreatedTimestampUtc { get; set; }

		[DataMember]
		public int Retries { get; set; }

		#region IFakeSeederAdapter
		public IList<NotificationDto> SeedFakeDtos()
		{
			return new List<NotificationDto>
			{
				new NotificationDto
				{
					Payload = new PayloadBaseDto
					{
						Id = "3441a80b-cf00-41f5-80f1-b069f1d3cda6"
					},
					Action = NotificationTypes.FixClientRequest,
					Tags = new List<NotificationTagDto>
					{
						new NotificationTagDto
						{
							Key = "userId",
							Value = "3441a80b-cf00-41f5-80f1-b069f1d3cda6"
						}
					},
					Recipients = new List<UserSummaryDto>
					{
						new UserSummaryDto
						{
							Id = Guid.Parse("3441a80b-cf00-41f5-80f1-b069f1d3cda6"),
							FirstName = "Jon",
							LastName = "Doe"
						}
					},
					Silent = true
				}
			};
		}
		#endregion
	}
}
