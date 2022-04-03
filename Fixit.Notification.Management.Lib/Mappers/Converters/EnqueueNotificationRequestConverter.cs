using AutoMapper;
using Fixit.Core.DataContracts.Notifications.Operations;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using System;
using System.Collections.Generic;

namespace Fixit.Notification.Management.Lib.Mappers.Converters
{
  public class EnqueueNotificationRequestConverter : ITypeConverter<EnqueueNotificationRequestDto, IList<NotificationCreateRequestDto>>
  {
    public IList<NotificationCreateRequestDto> Convert(EnqueueNotificationRequestDto source, IList<NotificationCreateRequestDto> destination, ResolutionContext context)
    {
      EnqueueNotificationRequestDto sourceEnqueueNotificationRequestDto = source;

      var notificationDocuments = new List<NotificationCreateRequestDto>();

      foreach (var recipient in sourceEnqueueNotificationRequestDto.RecipientUsers)
      {
        NotificationCreateRequestDto notificationDocument = context.Mapper.Map<EnqueueNotificationRequestDto, NotificationCreateRequestDto>(source);
        notificationDocument.RecipientUser = recipient;
        notificationDocument.Id = Guid.NewGuid();
        notificationDocument.IsTransient = source.IsTransient;

        notificationDocuments.Add(notificationDocument);
      }

      return notificationDocuments; 
    }
  }
}
