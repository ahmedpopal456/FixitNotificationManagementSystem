using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.Database;
using Fixit.Core.DataContracts.Fixes.Cost;
using Fixit.Core.DataContracts.Fixes.Details;
using Fixit.Core.DataContracts.Fixes.Enums;
using Fixit.Core.DataContracts.Fixes.Files;
using Fixit.Core.DataContracts.Fixes.Locations;
using Fixit.Core.DataContracts.Fixes.Schedule;
using Fixit.Core.DataContracts.Fixes.Tags;
using Fixit.Core.DataContracts.FixPlans;
using Fixit.Core.DataContracts.Seeders;
using Fixit.Core.DataContracts.Users;

namespace Fixit.Notification.Management.Lib.Models
{
    /// <summary>
    /// TODO: Move to DataContracts or Move to FMS.Lib
    /// </summary>
    [DataContract]
    public class FixDocument : DocumentBase, IFakeSeederAdapter<FixDocument>
    {

        [DataMember]
        public UserSummaryDto AssignedToCraftsman { get; set; }

        [DataMember]
        public IEnumerable<TagDto> Tags { get; set; }

        [DataMember]
        public IEnumerable<FixDetailsDto> Details { get; set; }

        [DataMember]
        public IEnumerable<FileDto> Images { get; set; }

        [DataMember]
        public FixLocationDto Location { get; set; }

        [DataMember]
        public IEnumerable<FixScheduleRangeDto> Schedule { get; set; }

        [DataMember]
        public FixCostRangeDto ClientEstimatedCost { get; set; }

        [DataMember]
        public float SystemCalculatedCost { get; set; }

        [DataMember]
        public FixCostEstimationDto CraftsmanEstimatedCost { get; set; }

        [DataMember]
        public long CreatedTimestampUtc { get; set; }

        [DataMember]
        public UserSummaryDto CreatedByClient { get; set; }

        [DataMember]
        public long UpdatedTimestampUtc { get; set; }

        [DataMember]
        public UserSummaryDto UpdatedByUser { get; set; }

        [DataMember]
        public FixStatuses Status { get; set; }

        [DataMember]
        public Guid ActivityHistoryId { get; set; }

        [DataMember]
        public Guid BillingActivityId { get; set; }

        [DataMember]
        public FixPlanSummaryDto PlanSummary { get; set; }

        #region SeedFakeDtos
        public new IList<FixDocument> SeedFakeDtos()
        {
            FixDocument firstFixDocument = new FixDocument
            {
                Details = new List<FixDetailsDto>()
                {
                    new FixDetailsDto()
                    {
                        Name = "Restore Brick Wall",
                        Description = "An area of the exposed brick wall in the dining space  is crumbling and cracked. Help is needed to restore the damaged area.",
                        Category = "Masonry",
                        Type = "Quick Fix"
                    }
                },
                Tags = new List<TagDto>
                {
                    new TagDto
                    {
                        Id = new Guid("8b418766-4a99-42a8-b6d7-9fe52b88ea98"),
                        Name = "Brick Wall"
                    }
                },
                Location = new FixLocationDto()
                {
                    Address = "1365 Rue Jean-Brillon",
                    City = "LaSalle",
                    Province = "Quebec",
                    PostalCode = "H8N 1R8"
                },
                Schedule = new List<FixScheduleRangeDto>()
                {
                    new FixScheduleRangeDto
                    {
                        StartTimestampUtc = 1617819166,
                        EndTimestampUtc = 1617822561
                    }
                },
                CreatedByClient = new UserSummaryDto()
                {
                    Id = new Guid("33c3aa05-ec00-418f-9072-7cdad5ec46e6"),
                    FirstName = "Frances",
                    LastName = "Alvarez"
                },
                UpdatedByUser = new UserSummaryDto()
                {
                    Id = new Guid("33c3aa05-ec00-418f-9072-7cdad5ec46e6"),
                    FirstName = "Frances",
                    LastName = "Alvarez"
                }

            };
            FixDocument secondFixDocument = null;

            return new List<FixDocument>
            {
                firstFixDocument,
                secondFixDocument
            };
        }
        #endregion
    }
}


