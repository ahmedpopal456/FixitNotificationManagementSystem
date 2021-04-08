using Fixit.Core.DataContracts.Users.Skills;
using System.Collections.Generic;

namespace Fixit.Notification.Management.Lib.Builders.Rules
{
    public class SkillComparer : IEqualityComparer<SkillDto>
    {
        public bool Equals(SkillDto x, SkillDto y)
        {
            if(x.Id == y.Id && x.Name == y.Name)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(SkillDto obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
