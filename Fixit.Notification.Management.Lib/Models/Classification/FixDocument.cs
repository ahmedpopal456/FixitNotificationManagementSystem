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
using Fixit.Core.DataContracts.Users;

namespace Fixit.Notification.Management.Lib.Models
{
	/// <summary>
	/// TODO: Move to DataContracts or Move to FMS.Lib
	/// </summary>
	[DataContract]
	public class FixDocument : DocumentBase
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
	}
}
