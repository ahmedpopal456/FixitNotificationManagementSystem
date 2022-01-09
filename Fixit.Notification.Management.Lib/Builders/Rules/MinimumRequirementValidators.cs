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
        public static bool IsEqual(SkillDto skill1, SkillDto skill2)
        {
            return skill1.Id == skill2.Id && skill1.Name == skill2.Name;
        }

        public static bool HasMinimumSkillRequirement(IEnumerable<SkillDto> skills, CraftsmenScores craftsmanScore, out int qualifiedSkillCount)
        {
            var qualifiedSkills = skills.ToList();
            var craftsmanSkills = craftsmanScore.UserDocument.Skills.ToList();  
            var skillDifference = qualifiedSkills.Except(craftsmanSkills, new SkillComparer()).ToList();
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
