using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Fixit.Core.DataContracts.Notifications.Enums;
using Fixit.Core.DataContracts.Notifications.Operations;
using Fixit.Core.DataContracts.Notifications.Payloads;
using Fixit.Notification.Management.Lib.Mappers.Converters;
using Fixit.Notification.Management.Lib.Models;
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
       .ForMember(installation => installation.Tags, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto != null && deviceInstallationUpsertRequestDto.Tags != default ? deviceInstallationUpsertRequestDto.Tags.Select(tag => $"{tag.Key}:{tag.Value}").ToList() : new List<string>()))
       .ForMember(installation => installation.Templates, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => new Dictionary<string, InstallationTemplate>()))
       .ReverseMap();

      CreateMap<NotificationTemplateBaseDto, InstallationTemplate>()
       .ForMember(installationTemplate => installationTemplate.Body, opts => opts.MapFrom(notificationTemplateBaseDto => notificationTemplateBaseDto != null ? notificationTemplateBaseDto.Body : default))
       .ForMember(installationTemplate => installationTemplate.Tags, opts => opts.MapFrom(notificationTemplateBaseDto => notificationTemplateBaseDto != null && notificationTemplateBaseDto.Tags != default ? notificationTemplateBaseDto.Tags.Select(tag => $"{tag.Key}:{tag.Value}").ToList() : new List<string>()))
       .ReverseMap();

      CreateMap<Installation, DeviceInstallationDto>()
       .ForMember(deviceInstallationDto => deviceInstallationDto.InstallationId, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.InstallationId))
       .ForMember(deviceInstallationDto => deviceInstallationDto.Platform, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.Platform))
       .ForMember(deviceInstallationDto => deviceInstallationDto.PushChannelToken, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.PushChannel))
       .ForMember(deviceInstallationDto => deviceInstallationDto.PushChannelTokenExpired, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.PushChannelExpired))
       .ForMember(deviceInstallationDto => deviceInstallationDto.Tags, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto != null && deviceInstallationUpsertRequestDto.Tags != null ? deviceInstallationUpsertRequestDto.Tags.Select(tag => new NotificationTagDto { Key = tag.Split(":", StringSplitOptions.None).FirstOrDefault(), Value = tag.Split(":", StringSplitOptions.None).LastOrDefault() }) : null))
       .ForMember(deviceInstallationDto => deviceInstallationDto.Templates, opts => opts.MapFrom(deviceInstallationUpsertRequestDto => deviceInstallationUpsertRequestDto.Templates))
       .ForMember(deviceInstallationDto => deviceInstallationDto.UserId, opts => opts.Ignore())
       .ReverseMap();

      #endregion

      #region Notifications 

      CreateMap<EnqueueNotificationRequestDto, IList<NotificationCreateRequestDto>>()
        .ConvertUsing<EnqueueNotificationRequestConverter>();

      CreateMap<EnqueueNotificationRequestDto, NotificationCreateRequestDto>()
        .ForMember(notificationDto => notificationDto.Title, opts => opts.MapFrom(notificationCreateRequestDto => notificationCreateRequestDto.Title))
        .ForMember(notificationDto => notificationDto.Message, opts => opts.MapFrom(notificationCreateRequestDto => notificationCreateRequestDto.Message))
        .ForMember(notificationDto => notificationDto.Payload, opts => opts.MapFrom(notificationCreateRequestDto => notificationCreateRequestDto.Payload))
        .ForMember(notificationDto => notificationDto.Silent, opts => opts.MapFrom(notificationCreateRequestDto => notificationCreateRequestDto.Silent))
        .ForMember(notificationDto => notificationDto.RecipientUser, opts => opts.Ignore())
        .ReverseMap();

      CreateMap<NotificationCreateRequestDto, NotificationDocument>()
       .ForMember(notificationDocument => notificationDocument.id, opts => opts.MapFrom(notificationCreateRequestDto => notificationCreateRequestDto.Id.ToString()))
       .ForMember(notificationDocument => notificationDocument.Title, opts => opts.MapFrom(notificationCreateRequestDto => notificationCreateRequestDto.Title))
       .ForMember(notificationDocument => notificationDocument.Message, opts => opts.MapFrom(notificationCreateRequestDto => notificationCreateRequestDto.Message))
       .ForMember(notificationDocument => notificationDocument.Payload, opts => opts.MapFrom(notificationCreateRequestDto => notificationCreateRequestDto.Payload))
       .ForMember(notificationDocument => notificationDocument.Silent, opts => opts.MapFrom(notificationCreateRequestDto => notificationCreateRequestDto.Silent))
       .ForMember(notificationDocument => notificationDocument.CreatedTimestampUtc, opts => opts.MapFrom(notificationCreateRequestDto => DateTimeOffset.UtcNow.ToUnixTimeSeconds()))
       .ForMember(notificationDocument => notificationDocument.UpdatedTimestampUtc, opts => opts.MapFrom(notificationCreateRequestDto => DateTimeOffset.UtcNow.ToUnixTimeSeconds()))
       .ForMember(notificationDocument => notificationDocument.EntityId, opts => opts.MapFrom(notificationCreateRequestDto => notificationCreateRequestDto.RecipientUser.Id))
       .ForMember(notificationDocument => notificationDocument.Status, opts => opts.MapFrom(notificationCreateRequestDto => NotificationStatus.Sent))
       .ForMember(notificationDocument => notificationDocument.RecipientUser, opts => opts.MapFrom(notificationCreateRequestDto => notificationCreateRequestDto.RecipientUser))
       .ReverseMap();

      CreateMap<FixDocument, FixAssignmentValidationDto>()
       .ForMember(fixAssignmentValidationDto => fixAssignmentValidationDto.Id, opts => opts.MapFrom(fixDocument => fixDocument.id))
       .ForMember(fixAssignmentValidationDto => fixAssignmentValidationDto.AssignedToCraftsman, opts => opts.MapFrom(fixDocument => fixDocument.AssignedToCraftsman))
       .ForMember(fixAssignmentValidationDto => fixAssignmentValidationDto.CraftsmanEstimatedCost, opts => opts.MapFrom(fixDocument => fixDocument.CraftsmanEstimatedCost))
       .ForMember(fixAssignmentValidationDto => fixAssignmentValidationDto.Schedule, opts => opts.MapFrom(fixDocument => fixDocument.Schedule))
       .ReverseMap();

      #endregion
    }
  }
}
