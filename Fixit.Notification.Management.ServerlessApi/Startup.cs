using AutoMapper;
using Fixit.Core.Database;
using Fixit.Core.Database.Mediators;
using Fixit.Core.DataContracts.Decorators.Extensions;
using Fixit.Core.DataContracts.Events;
using Fixit.Core.Storage;
using Fixit.Core.Storage.Storage.Queue.Mediators;
using Fixit.Notification.Management.Lib.Constants;
using Fixit.Notification.Management.Lib.Extensions;
using Fixit.Notification.Management.Lib.Mappers;
using Fixit.Notification.Management.ServerlessApi;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using static Fixit.Notification.Management.Lib.NotificationAssemblyInfo;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Fixit.Notification.Management.ServerlessApi
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
      });
      var mapper = mapperConfig.CreateMapper();

      StorageFactory storageFactory = new StorageFactory(_configuration["FIXIT-NMS-STORAGEACCOUNT-CS"]);
      DatabaseFactory databaseFactory = new DatabaseFactory(_configuration["FIXIT-NMS-DB-EP"], _configuration["FIXIT-NMS-DB-KEY"]);

      builder.AddFixitNotificationServices();
      builder.Services.AddFixitCoreDecoratorServices();
      builder.Services.AddSingleton<IDatabaseMediator>(databaseFactory.CreateCosmosClient());
      builder.Services.AddSingleton<IMapper>(mapperConfig.CreateMapper());
      builder.Services.AddSingleton<IQueueServiceClientMediator>(storageFactory.CreateQueueServiceClientMediator());
      builder.Services.AddSingleton<INotificationHubClient>(serviceProvider =>
      {
        var notificationHubName = _configuration["FIXIT-NMS-ANH-NAME"];
        var notificationHubConnectionString = _configuration["FIXIT-NMS-ANH-CS"];

        return NotificationHubClient.CreateClientFromConnectionString(notificationHubConnectionString, notificationHubName);
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
