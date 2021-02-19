﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Fixit.Core.DataContracts.Users;
using Fixit.Notification.Management.Lib.Models.Notifications.Enums;
using Fixit.Notification.Management.Lib.Models.Notifications.Payloads;
using Fixit.Notification.Management.Lib.Seeders;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests
{
  [DataContract]
  public class EnqueueNotificationRequestDto : IDtoValidator, IFakeSeederAdapter<EnqueueNotificationRequestDto>
  {
    [DataMember]
    public PayloadBaseDto Payload { get; set; }

    [DataMember]
    public NotificationTypes Action { get; set; }

    [DataMember]
    public IList<NotificationTagDto> Tags { get; set; }

    [DataMember]
    public IEnumerable<UserSummaryDto> Recipients { get; set; }

    [DataMember]
    public bool Silent { get; set; }

    [DataMember]
    public int Retries { get; set; }

    #region IDtoValidator
    public bool Validate()
    {
      bool isValid = (Payload != null)
                     || ((Tags != null && Tags.Any()) || (Recipients != null && Recipients.Any()))
                     && (Enum.IsDefined(typeof(NotificationTypes), Action));

      return isValid;
    }
    #endregion

    #region IFakeSeederAdapter
    public IList<EnqueueNotificationRequestDto> SeedFakeDtos()
    {
      return new List<EnqueueNotificationRequestDto>
      {
        new EnqueueNotificationRequestDto
				{
          Payload = new PayloadBaseDto
					{
            Id = Guid.Parse("3441a80b-cf00-41f5-80f1-b069f1d3cda6")
          },
          Action = NotificationTypes.FixClientRequest
        }
      };
    }
		#endregion
	}
}