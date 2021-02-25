using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fixit.Core.DataContracts.Fixes.Categories;
using Fixit.Core.DataContracts.Fixes.Cost;
using Fixit.Core.DataContracts.Fixes.Details;
using Fixit.Core.DataContracts.Fixes.Files;
using Fixit.Core.DataContracts.Fixes.Locations;
using Fixit.Core.DataContracts.Fixes.Schedule;
using Fixit.Core.DataContracts.Fixes.Types;
using Fixit.Core.DataContracts.Users;

namespace Fixit.Notification.Management.Lib.Models.Notifications.Payloads
{
	// TODO: Move to FixManagementSystem

	[DataContract]
	public class FixAssignmentValidationDto : PayloadBaseDto
	{
		[DataMember]
		public UserSummaryDto AssignedToCraftsman { get; set; }

		[DataMember]
		public FixCostRangeDto ClientBudget { get; set; }

		[DataMember]
		public float SystemCalculatedCost { get; set; }

		[DataMember]
		public FixCostEstimationDto CraftsmanEstimatedCost { get; set; }

		[DataMember]
		public IList<FixScheduleRangeDto> Schedule { get; set; }

		[DataMember]
		public FixCategoryDto FixCategory { get; set; }

		[DataMember]
		public FixTypeDto FixType { get; set; }

		[DataMember]
		public FixLocationDto Location { get; set; }

		[DataMember]
		public IEnumerable<FileDto> Images { get; set; }

		[DataMember]
		public FixDetailsDto FixDetails { get; set; }
	}
}
