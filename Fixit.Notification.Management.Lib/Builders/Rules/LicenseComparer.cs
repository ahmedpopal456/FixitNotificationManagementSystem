using Fixit.Core.DataContracts.Users.Skill;
using System.Collections.Generic;

namespace Fixit.Notification.Management.Lib.Builders.Rules
{
  public class LicenseComparer : IEqualityComparer<UserLicenseDto>
  {
    public bool Equals(UserLicenseDto x, UserLicenseDto y)
    {
      if (x.Id == y.Id && x.Id == y.Id)
      {
        return true;
      }
      return false;
    }

    public int GetHashCode(UserLicenseDto obj)
    {
      return obj.Id.GetHashCode();
    }
  }
}
