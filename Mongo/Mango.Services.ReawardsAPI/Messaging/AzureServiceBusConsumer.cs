using Azure.Messaging.ServiceBus;
using Mango.Services.RewardsAPI.Message;
using Mango.Services.RewardsAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardsAPI.Messaging
{

    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardSubscription;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _rewardProcessor;
        // critical
        // we want directly the service and not the interface
        // that is registered with singleton implementation
        private readonly RewardService _rewardService;



        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
                _configuration = configuration;
            _rewardService = rewardService;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionStrings");

            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");

            orderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);

            //in this way proccessor lister to subscription
            //which is inside topic
            _rewardProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardSubscription);
        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            
            //critical
            //without this line processing will never starts
            await _rewardProcessor.StartProcessingAsync();
        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive the message
            var message=args.Message;
            
            var body=Encoding.UTF8.GetString(message.Body);

            RewardsMessage obj =JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                //todo try to log email
                await _rewardService.UpdateRewards(obj);
                await args.CompleteMessageAsync(args.Message);

            }catch (Exception ex) {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {   
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();
            await _rewardProcessor.DisposeAsync();
        }
    }
}
