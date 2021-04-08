using AutoMapper;
using Fixit.Core.DataContracts.Users;
using Fixit.Core.DataContracts.Users.Operations.Ratings;
using Fixit.Notification.Management.Lib.Models;
using System.Collections.Generic;

namespace Fixit.Notification.Management.Lib.Mappers
{
	public class FixClassificationMapper : Profile
	{
		public FixClassificationMapper()
		{

			CreateMap<UserDocument, UserSummaryDto>()
				.ForMember(userSummary => userSummary.Id, opts => opts.MapFrom(userDocument => userDocument.id))
				.ForMember(userSummary => userSummary.FirstName, opts => opts.MapFrom(userDocument => userDocument.FirstName))
				.ForMember(userSummary => userSummary.LastName, opts => opts.MapFrom(userDocument => userDocument.LastName))
				.ForMember(userSummary => userSummary.ProfilePictureUrl, opts => opts.MapFrom(userDocument => userDocument.ProfilePictureUrl))
				.ForMember(userSummary => userSummary.Role, opts => opts.MapFrom(userDocument => userDocument.Role))
				.ForMember(userSummary => userSummary.Status, opts => opts.MapFrom(userDocument => userDocument.Status))
				.ReverseMap();

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
				.ForMember(userDocument => userDocument.Availability, opts => opts.MapFrom(userDto => userDto.Availability))
				.ForMember(userDocument => userDocument.Skills, opts => opts.MapFrom(userDto => userDto != null ? userDto.Skills : default))
				.ReverseMap();

			CreateMap<RatingListResponseDto, RatingListDocument>()
				.ForMember(ratingListDocument => ratingListDocument.id, opts => opts.MapFrom(ratinListDto => ratinListDto.Id))
				.ForMember(ratingListDocument => ratingListDocument.EntityId, opts => opts.MapFrom(ratinListDto => ratinListDto.EntityId))
				.ForMember(ratingListDocument => ratingListDocument.AverageRating, opts => opts.MapFrom(ratinListDto => ratinListDto.AverageRating))
				.ForMember(ratingListDocument => ratingListDocument.ReviewCount, opts => opts.MapFrom(ratinListDto => ratinListDto.ReviewCount))
				.ReverseMap();

		}

	
	}
}
