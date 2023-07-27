using Mango.Services.EmailAPI.Messaging;
using System.Reflection.Metadata;

namespace Mango.Services.EmailAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer azureServiceBus { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            azureServiceBus=app.ApplicationServices.GetService<AzureServiceBusConsumer>();

            //notify the lifetime of application
            //when it starts or stop
            var hostApplicationLife=app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopping.Register(OnStop);

            return app;
        }

        private static void OnStop()
        {
           azureServiceBus.Stop();
        }

        private static void OnStart()
        {
            azureServiceBus.Start();
        }
    }
}
