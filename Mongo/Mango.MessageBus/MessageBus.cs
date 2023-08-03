using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.MessageBus
{
    //todo add comments
    //todo connectionString not here but in config file
    public class MessageBus : IMessageBus
    {
        private string connectionString = "your connection string, and i have to place it to appsettings";
        public async Task PublishMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender=client.CreateSender(topic_queue_Name);

            var jsonMessge=JsonConvert.SerializeObject(message);

            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessge))
            {
                CorrelationId=Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(serviceBusMessage);

            await client.DisposeAsync();
        }
    }
}
