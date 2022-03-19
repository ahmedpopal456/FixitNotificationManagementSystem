using Fixit.Core.DataContracts.Users.Skill;
using Fixit.Notification.Management.Lib.Models;
using Fixit.Notification.Management.Lib.Models.Classification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fixit.Notification.Management.Lib.Builders.Rules
{
  public static class MinimumRequirementValidators
  {
    public static bool IsEqual(UserLicenseDto skill1, UserLicenseDto skill2)
    {
      return skill1.Id == skill2.Id && skill1.Id == skill2.Id;
    }

    public static bool HasMinimumLicenseRequirement(IEnumerable<UserLicenseDto> userLicenseDtos, CraftsmenScores craftsmanScore, out int qualifiedSkillCount)
    {
      var qualifiedSkills = userLicenseDtos.ToList();
      var craftsmanSkills = craftsmanScore.UserDocument.Licenses.ToList();
      var skillDifference = qualifiedSkills.Except(craftsmanSkills, new LicenseComparer()).ToList();
      qualifiedSkillCount = qualifiedSkills.Count - skillDifference.Count;
      var result = qualifiedSkillCount > 0;

      return result;
    }

    public static bool HasMinimumLocationRequirement(int maximumDistance, int maximumDuration, GoogleDistanceMatrixResponse responseFromServer, out double distance, out double duration)
    {
      distance = Convert.ToDouble(responseFromServer.Distance.Value);
      duration = Convert.ToDouble(responseFromServer.Duration.Value);
      var result = distance <= maximumDistance && duration <= maximumDuration;

      return result;

    }
  }
}
