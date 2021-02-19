using System;
using System.Collections.Generic;
using System.Text;
using Fixit.Notification.Management.Lib.Models.Notifications.Operations.Requests;

namespace Fixit.Notification.Management.Lib.Seeders
{
	public class FakeDtoSeederFactory : IFakeSeederFactory
	{
		public IList<T> CreateSeederFactory<T>(IFakeSeederAdapter<T> fakeSeederAdapter) where T : class
		{
			return fakeSeederAdapter.SeedFakeDtos();
		}
	}
}
