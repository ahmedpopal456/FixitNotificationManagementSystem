using System;
using System.Collections.Generic;
using System.Text;

namespace Fixit.Notification.Management.Lib.Seeders
{
	public interface IFakeSeederAdapter<T>
	{
		public IList<T> SeedFakeDtos();
	}
}
