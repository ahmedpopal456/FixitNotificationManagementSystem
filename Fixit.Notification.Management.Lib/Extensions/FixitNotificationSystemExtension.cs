using AutoMapper;
using Fixit.Core.DataContracts;
using Fixit.Core.DataContracts.Decorators.Exceptions;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Mediators.Internal;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Fixit.Notification.Management.Lib.Extensions
{
	public static class FixitNotificationSystemExtension
	{
		public static void AddFixitNotificationServices(this IFunctionsHostBuilder builder)
		{
			builder.Services.AddSingleton<IExceptionDecorator<OperationStatus>, OperationStatusExceptionDecorator>();
			builder.Services.AddSingleton<INotificationMediator, NotificationMediator>();
			builder.Services.AddTransient<INotificationInstallationMediator, NotificationInstallationMediator>();
		}
	}
}
