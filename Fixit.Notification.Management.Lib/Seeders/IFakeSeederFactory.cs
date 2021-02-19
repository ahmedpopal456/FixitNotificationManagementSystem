using System;
using System.Collections.Generic;
using System.Text;

namespace Fixit.Notification.Management.Lib.Seeders
{
	public interface IFakeSeederFactory
	{
		public IList<T> CreateSeederFactory<T>(IFakeSeederAdapter<T> fakeSeederAdapter) where T: class;
	}
}
