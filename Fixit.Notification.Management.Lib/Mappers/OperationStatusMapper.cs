using AutoMapper;
using Fixit.Core.DataContracts;
using Fixit.Notification.Management.Lib.Models.Notifications.Installations;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Responses;
using System;
using System.Net;

namespace Fixit.Notification.Management.Lib.Mappers
{
  public class OperationStatusMapper : Profile
  {
    public OperationStatusMapper()
    {
      CreateMap<OperationStatus, OperationStatusWithObject<DeviceInstallationDocument>>()
        .ForMember(operationStatusWithObject => operationStatusWithObject.IsOperationSuccessful, opts => opts.MapFrom(operationStatus => operationStatus != null ? operationStatus.IsOperationSuccessful : false))
        .ForMember(operationStatusWithObject => operationStatusWithObject.Error, opts => opts.Ignore())
        .ForMember(operationStatusWithObject => operationStatusWithObject.OperationException, opts => opts.Condition(operationStatus => operationStatus != null));

      CreateMap<OperationStatus, OperationStatusWithObject<NotificationsDeleteResponse>>()
        .ForMember(operationStatusWithObject => operationStatusWithObject.IsOperationSuccessful, opts => opts.MapFrom(operationStatus => operationStatus != null ? operationStatus.IsOperationSuccessful : false))
        .ForMember(operationStatusWithObject => operationStatusWithObject.Error, opts => opts.Ignore())
        .ForMember(operationStatusWithObject => operationStatusWithObject.OperationException, opts => opts.Condition(operationStatus => operationStatus != null));

      CreateMap<OperationStatus, OperationStatusWithObject<NotificationStatusUpdateResponseDto>>()
        .ForMember(operationStatusWithObject => operationStatusWithObject.IsOperationSuccessful, opts => opts.MapFrom(operationStatus => operationStatus != null ? operationStatus.IsOperationSuccessful : false))
        .ForMember(operationStatusWithObject => operationStatusWithObject.Error, opts => opts.Ignore())
        .ForMember(operationStatusWithObject => operationStatusWithObject.OperationException, opts => opts.Condition(operationStatus => operationStatus != null));
    }
  }
}
