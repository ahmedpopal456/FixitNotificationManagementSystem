using AutoMapper;
using Fixit.Core.DataContracts.Users;
using Fixit.Notification.Management.Lib.Models;

namespace Fixit.Notification.Management.Lib.Mappers
{
	public class FixClassificationMapper : Profile
	{
		public FixClassificationMapper()
		{
			#region UserSummary 

			CreateMap<UserDocument, UserSummaryDto>()
				.ForMember(userSummary => userSummary.Id, opts => opts.MapFrom(userDocument => userDocument.id))
				.ForMember(userSummary => userSummary.FirstName, opts => opts.MapFrom(userDocument => userDocument.FirstName))
				.ForMember(userSummary => userSummary.LastName, opts => opts.MapFrom(userDocument => userDocument.LastName))
				.ForMember(userSummary => userSummary.ProfilePictureUrl, opts => opts.MapFrom(userDocument => userDocument.ProfilePictureUrl))
				.ForMember(userSummary => userSummary.Role, opts => opts.MapFrom(userDocument => userDocument.Role))
				.ForMember(userSummary => userSummary.Status, opts => opts.MapFrom(userDocument => userDocument.Status))
				.ReverseMap();

			#endregion

			CreateMap<UserDto, UserDocument>()
				.ForMember(userDocument => userDocument.id, opts => opts.MapFrom(userDto => userDto.Id))
				.ForMember(userDocument => userDocument.UserPrincipalName, opts => opts.MapFrom(userDto => userDto.UserPrincipalName))
				.ForMember(userDocument => userDocument.ProfilePictureUrl, opts => opts.MapFrom(userDto => userDto.ProfilePictureUrl))
				.ForMember(userDocument => userDocument.FirstName, opts => opts.MapFrom(userDto => userDto.FirstName))
				.ForMember(userDocument => userDocument.LastName, opts => opts.MapFrom(userDto => userDto.LastName))
				.ForMember(userDocument => userDocument.State, opts => opts.MapFrom(userDto => userDto.State))
				.ForMember(userDocument => userDocument.Address, opts => opts.MapFrom(userDto => userDto.Address))
				.ForMember(userDocument => userDocument.Role, opts => opts.MapFrom(userDto => userDto.Role))
				.ForMember(userDocument => userDocument.Status, opts => opts.MapFrom(userDto => userDto.Status))
				.ReverseMap();


		}
	}
}
