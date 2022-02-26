using AutoMapper;
using Fixit.Core.Database;
using Fixit.Core.Database.Mediators;
using Fixit.Core.Storage;
using Fixit.Notification.Management.Lib.Extensions;
using Fixit.Notification.Management.Lib.Mappers;
using Fixit.Notification.Management.Triggers;
using Fixit.Notification.Management.Lib.Mediators;
using Fixit.Notification.Management.Lib.Mediators.Internal;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Fixit.Core.Networking.Extensions;
using Fixit.Core.Networking.Local.UMS;
using Fixit.Core.Networking.Local.MDM;
using Fixit.Core.DataContracts.Decorators.Extensions;
using Fixit.Core.Storage.Storage.Queue.Mediators;
using static Fixit.Notification.Management.Lib.NotificationAssemblyInfo;
using Fixit.Core.DataContracts.Events;
using Fixit.Notification.Management.Lib.Constants;
using System.Collections.Generic;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Fixit.Notification.Management.Triggers
{
  public class Startup : FunctionsStartup
  {
    private IConfiguration _configuration;

    public override void Configure(IFunctionsHostBuilder builder)
    {
      _configuration = (IConfiguration)builder.Services.BuildServiceProvider()
                               .GetService(typeof(IConfiguration));

      var mapperConfig = new MapperConfiguration(mc =>
      {
        mc.AddProfile(new FixitNotificationMapper());
        mc.AddProfile(new OperationStatusMapper());
        mc.AddProfile(new FixClassificationMapper());
      });
      var mapper = mapperConfig.CreateMapper();
      StorageFactory storageFactory = new StorageFactory(_configuration["FIXIT-NMS-STORAGEACCOUNT-CS"]);
      DatabaseFactory databaseFactory = new DatabaseFactory(_configuration["FIXIT-NMS-DB-EP"], _configuration["FIXIT-NMS-DB-KEY"]);

      builder.Services.AddLogging();
      builder.AddFixitNotificationServices();
      builder.Services.AddSingleton<IDatabaseMediator>(databaseFactory.CreateCosmosClient());
      builder.Services.AddSingleton<IMapper>(mapperConfig.CreateMapper());
      builder.Services.AddSingleton<IQueueServiceClientMediator>(storageFactory.CreateQueueServiceClientMediator());
      builder.Services.AddSingleton<INotificationHubClient>(serviceProvider =>
      {
        var notificationHubName = _configuration["FIXIT-NMS-ANH-NAME"];
        var notificationHubConnectionString = _configuration["FIXIT-NMS-ANH-CS"];

        return NotificationHubClient.CreateClientFromConnectionString(notificationHubConnectionString, notificationHubName);
      });

      builder.Services.AddFixitCoreDecoratorServices();
      builder.Services.AddUmServices(_configuration["FIXIT-UMS-SERVICE-EP"]);
      builder.Services.AddMdmServices(_configuration["FIXIT-MDM-SERVICE-EP"]);
      builder.Services.AddSingleton<IFixClassificationMediator, FixClassificationMediator>(serviceProvider =>
      {
        var mapper = serviceProvider.GetService<IMapper>();
        var httpUmClient = serviceProvider.GetService<IFixUmsHttpClient>();
        var httpMdmClient = serviceProvider.GetService<IFixMdmHttpClient>();
        var configuration = serviceProvider.GetService<IConfiguration>();

        return new FixClassificationMediator(mapper, httpUmClient, httpMdmClient, configuration);
      });

      builder.Services.AddTransient<EventGridTopicServiceClientResolver>(serviceProvider => key =>
      {
        return key switch
        {
          NotificationEvents.OnNotificationEnqueued => AzureEventGridTopicServiceClientFactory.CreateEventGridTopicServiceClient(_configuration["FIXIT-NMS-EG-ONNOTIFICATIONSENQUEUE-TE"], _configuration["FIXIT-NMS-EG-ONNOTIFICATIONSENQUEUE-TK"]),
          NotificationEvents.OnNotificationDispatchedForStorage => AzureEventGridTopicServiceClientFactory.CreateEventGridTopicServiceClient(_configuration["FIXIT-NMS-EG-ONNOTIFICATIONDISPATCHEDFORSTORAGE-TE"], _configuration["FIXIT-NMS-EG-ONNOTIFICATIONDISPATCHEDFORSTORAGE-TK"]),
          NotificationEvents.OnNotificationStored => AzureEventGridTopicServiceClientFactory.CreateEventGridTopicServiceClient(_configuration["FIXIT-NMS-EG-ONNOTIFICATIONSTORED-TE"], _configuration["FIXIT-NMS-EG-ONNOTIFICATIONSTORED-TK"]),
          _ => throw new KeyNotFoundException()
        };
      });
    }
  }
}
