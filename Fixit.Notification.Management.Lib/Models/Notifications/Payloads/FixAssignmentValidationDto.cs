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
  public class FixAssignmentValidationDto
  {
    [DataMember]
    public Guid Id { get; set; }

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
    public FixCategoryDto FixCategory { get; }

    [DataMember]
    public FixTypeDto FixType { get; }

    [DataMember]
    public FixLocationDto Location { get; }

    [DataMember]
    public IEnumerable<FileDto> Images { get; }

    [DataMember]
    public FixDetailsDto FixDetails { get; }
  }
}
