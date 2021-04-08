using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.Database;
using Fixit.Core.DataContracts.Seeders;
using Fixit.Core.DataContracts.Users;
using Fixit.Core.DataContracts.Users.Address;
using Fixit.Core.DataContracts.Users.Availabilities;
using Fixit.Core.DataContracts.Users.Documents;
using Fixit.Core.DataContracts.Users.Enums;
using Fixit.Core.DataContracts.Users.Profile;
using Fixit.Core.DataContracts.Users.Skills;

namespace Fixit.Notification.Management.Lib.Models
{
    [DataContract]
    public class UserDocument : DocumentBase, IFakeSeederAdapter<UserDocument>
    {
        [DataMember]
        public string UserPrincipalName { get; set; }

        [DataMember]
        public string ProfilePictureUrl { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public UserState State { get; set; }

        [DataMember]
        public AddressDto Address { get; set; }

        [DataMember]
        public UserRole Role { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        [DataMember]
        public UserStatusDto Status { get; set; }

        [DataMember]
        public string TelephoneNumber { get; set; }

        [DataMember]
        public UserAvailabilityDto Availability { get; set; }

        [DataMember]
        public IEnumerable<SkillDto> Skills { get; set; }

        [DataMember]
        public long CreatedTimestampsUtc { get; set; }

        [DataMember]
        public long UpdatedTimestampsUtc { get; set; }

        [DataMember]
        public IEnumerable<DocumentSummaryDto> Documents { get; set; }

        #region SeedFakeDtos
        public new IList<UserDocument> SeedFakeDtos()
        {
            UserDocument firstUserDocument = new UserDocument
            {
                ProfilePictureUrl = "something.something/something.png",
                FirstName = "John",
                LastName = "Doe",
                UserPrincipalName = "johnDoe@test.com",
                Role = UserRole.Craftsman,
                Address = new AddressDto()
                {
                    Address = "123 Something",
                    City = "Montreal",
                    Province = "Quebec",
                    Country = "Canada",
                    PostalCode = "A1A 1A1",
                    PhoneNumber = "514-123-4567"
                },
                Skills = new List<SkillDto>()
                {
                    new SkillDto
                    {
                        Id = new Guid("ce59667a-5df5-4fa8-907f-80637ece426d"),
                        Name = "Small Concrete Works"
                    }
                },
                Availability = new UserAvailabilityDto
                {
                    Type = AvailabilityType.Always,
                    Schedule = new List<DayAvailabilityDto>()
                    {
                        new DayAvailabilityDto
                        {
                            DayName = DayOfWeek.Sunday,
                            BusinessHours = new List<BusinessHoursRangeDto>
                            {
                                new BusinessHoursRangeDto
                                {
                                    StartTimestampUtc = 0,
                                    EndTimestampUtc = 864000000000
                                }
                            }
                        },
                        new DayAvailabilityDto
                        {
                            DayName = DayOfWeek.Monday,
                            BusinessHours = new List<BusinessHoursRangeDto>
                            {
                                new BusinessHoursRangeDto
                                {
                                    StartTimestampUtc = 0,
                                    EndTimestampUtc = 864000000000
                                }
                            }
                        },
                        new DayAvailabilityDto
                        {
                            DayName = DayOfWeek.Tuesday,
                            BusinessHours = new List<BusinessHoursRangeDto>
                            {
                                new BusinessHoursRangeDto
                                {
                                    StartTimestampUtc = 0,
                                    EndTimestampUtc = 864000000000
                                }
                            }
                        }
                        ,
                        new DayAvailabilityDto
                        {
                            DayName = DayOfWeek.Wednesday,
                            BusinessHours = new List<BusinessHoursRangeDto>
                            {
                                new BusinessHoursRangeDto
                                {
                                    StartTimestampUtc = 0,
                                    EndTimestampUtc = 864000000000
                                }
                            }
                        }
                        ,
                        new DayAvailabilityDto
                        {
                            DayName = DayOfWeek.Thursday,
                            BusinessHours = new List<BusinessHoursRangeDto>
                            {
                                new BusinessHoursRangeDto
                                {
                                    StartTimestampUtc = 0,
                                    EndTimestampUtc = 864000000000
                                }
                            }
                        }
                        ,
                        new DayAvailabilityDto
                        {
                            DayName = DayOfWeek.Friday,
                            BusinessHours = new List<BusinessHoursRangeDto>
                            {
                                new BusinessHoursRangeDto
                                {
                                    StartTimestampUtc = 0,
                                    EndTimestampUtc = 864000000000
                                }
                            }
                        }
                        ,
                        new DayAvailabilityDto
                        {
                            DayName = DayOfWeek.Saturday,
                            BusinessHours = new List<BusinessHoursRangeDto>
                            {
                                new BusinessHoursRangeDto
                                {
                                    StartTimestampUtc = 0,
                                    EndTimestampUtc = 864000000000
                                }
                            }
                        }
                    }
                }
            };
            UserDocument secondUserDocument = null;

            return new List<UserDocument>
            {
                firstUserDocument,
                secondUserDocument
            };
        }
    }
    #endregion
}
