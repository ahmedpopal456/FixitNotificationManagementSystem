using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Fixit.Notification.Management.Lib.Models.Notifications;
using Fixit.Notification.Management.Lib.Models.Notifications.Installations;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;
using Fixit.Notification.Management.Lib.Models.Notifications.Templates;
using Microsoft.Azure.NotificationHubs;

namespace Fixit.Notification.Management.Lib.Mappers
{
  public class FixitNotificationMapper : Profile
  {
    public FixitNotificationMapper()
    {

      #region Device Installations 

      CreateMap<DeviceInstallationDto, DeviceInstallationDocument>()
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.EntityId, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.UserId))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.id, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.InstallationId))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.PushChannelTokenExpired, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.PushChannelTokenExpired))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.InstalledTimestampUtc, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.InstalledTimestampUtc))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.Platform, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.Platform))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.PushChannelToken, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.PushChannelToken))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.UpdatedTimestampUtc, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.UpdatedTimestampUtc))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.Tags, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.Tags))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.Templates, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.Templates))
        .ReverseMap();

      CreateMap<DeviceInstallationUpsertRequestDto, DeviceInstallationDocument>()
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.EntityId, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.UserId))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.id, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.InstallationId))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.InstalledTimestampUtc, opts => opts.Ignore())
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.Platform, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.Platform))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.PushChannelToken, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.PushChannelToken))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.UpdatedTimestampUtc, opts => opts.Ignore())
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.PushChannelTokenExpired, opts => opts.Ignore())
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.Tags, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.Tags))
        .ForMember(deviceInstallationDocument => deviceInstallationDocument.Templates, opts => opts.MapFrom(deviceInstallationDto => deviceInstallationDto.Templates))
        .ReverseMap();
      
      CreateMap<DeviceInstallationUpsertRequestDto, Installation>()
        .ForMember(installation => installation.InstallationId, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.InstallationId))
        .ForMember(installation => installation.Platform, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.Platform))
        .ForMember(installation => installation.PushChannel, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.PushChannelToken))
        .ForMember(installation => installation.Tags, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto != null && deviceInstallationUpsertRequestDto.Tags != null ? deviceInstallationUpsertRequestDto.Tags.Select(item => $"{item.Key}:{item.Value}") : new List<string>()))
        .ForMember(installation => installation.Templates, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => new Dictionary<string, Installation>()))
        .ReverseMap();

      CreateMap<NotificationTemplateBaseDto, InstallationTemplate>()
        .ForMember(installationTemplate => installationTemplate.Body, opts => opts.MapFrom(notificationTemplateBaseDto => notificationTemplateBaseDto.Body))
        .ForMember(installationTemplate => installationTemplate.Tags, opts => opts.MapFrom(notificationTemplateBaseDto => notificationTemplateBaseDto != null && notificationTemplateBaseDto.Tags != null ? notificationTemplateBaseDto.Tags.Select(item => $"{item.Key}:{item.Value}") : new List<string>()))
        .ReverseMap();

      CreateMap<Installation, DeviceInstallationDto>()
        .ForMember(deviceInstallationDto => deviceInstallationDto.InstallationId, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.InstallationId))
        .ForMember(deviceInstallationDto => deviceInstallationDto.Platform, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.Platform))
        .ForMember(deviceInstallationDto => deviceInstallationDto.PushChannelToken, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.PushChannel))
        .ForMember(deviceInstallationDto => deviceInstallationDto.PushChannelTokenExpired, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.PushChannelExpired))
        .ForMember(deviceInstallationDto => deviceInstallationDto.Tags, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto != null && deviceInstallationUpsertRequestDto.Tags != null ? deviceInstallationUpsertRequestDto.Tags.Select(tag => KeyValuePair.Create<string,string>(tag.Split(":", StringSplitOptions.None).FirstOrDefault(), tag.Split(":", StringSplitOptions.None).LastOrDefault())) : null))
        .ForMember(deviceInstallationDto => deviceInstallationDto.Templates, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.Templates))
        .ForMember(deviceInstallationDto => deviceInstallationDto.UserId, opts => opts.Ignore())
        .ReverseMap();

      #endregion

      #region Notifications 

      CreateMap<EnqueueNotificationRequestDto, NotificationDto>()
        .ForMember(notificationDto => notificationDto.Action, opts => opts.MapFrom(enqueueNotificationRequestDto => enqueueNotificationRequestDto.Action))
        .ForMember(notificationDto => notificationDto.Payload, opts => opts.MapFrom(enqueueNotificationRequestDto => enqueueNotificationRequestDto.Payload))
        .ForMember(notificationDto => notificationDto.Recipients, opts => opts.MapFrom(enqueueNotificationRequestDto => enqueueNotificationRequestDto.Recipients))
        .ForMember(notificationDto => notificationDto.Tags, opts => opts.MapFrom(enqueueNotificationRequestDto => enqueueNotificationRequestDto.Tags))
        .ForMember(notificationDto => notificationDto.Silent, opts => opts.MapFrom(enqueueNotificationRequestDto => enqueueNotificationRequestDto.Silent))
        .ForMember(notificationDto => notificationDto.Retries, opts => opts.MapFrom(enqueueNotificationRequestDto => enqueueNotificationRequestDto.Retries))
        .ForMember(notificationDto => notificationDto.CreatedTimestampUtc, opts => opts.Ignore())
        .ReverseMap();

      #endregion
    }
  }
}
