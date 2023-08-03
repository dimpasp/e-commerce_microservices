using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    //todo add comments
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registerUserQueue;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedEmailSubscription;
        private readonly IConfiguration _configuration;

        // critical
        // we want directly the service and not the interface
        // that is registered with singleton implementation
        private readonly EmailService _emailService;

        // listening queue for new messages
        //this is for the email about products
        private ServiceBusProcessor _serviceBusProcessor;
        //this is for register user
        private ServiceBusProcessor _registerUserProcessor;
        //this is for subscription
        private ServiceBusProcessor _emailOrderPlacedSubscription;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
                _configuration = configuration;
            _emailService = emailService;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionStrings");

            emailCartQueue= _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic ");

            orderCreatedEmailSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");


            var client = new ServiceBusClient(serviceBusConnectionString);

            _serviceBusProcessor=client.CreateProcessor(emailCartQueue);

            _registerUserProcessor = client.CreateProcessor(registerUserQueue);

            _emailOrderPlacedSubscription = client.CreateProcessor(orderCreatedTopic, orderCreatedEmailSubscription);

        }

        public async Task Start()
        {
            _serviceBusProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _serviceBusProcessor.ProcessErrorAsync += ErrorHandler;
            //critical
            //without this line processing will never starts
            await _serviceBusProcessor.StartProcessingAsync();


            //add this for the new queue about register user
            _registerUserProcessor.ProcessMessageAsync += OnUserRegisterReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();

            //add one more process for email subscription
            _emailOrderPlacedSubscription.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            _emailOrderPlacedSubscription.ProcessErrorAsync += ErrorHandler;
            await _emailOrderPlacedSubscription.StartProcessingAsync();
        }

        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive the message
            var message = args.Message;

            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage obj = JsonConvert.DeserializeObject<RewardsMessage>(body);
           

            try
            {
                //todo try to log email
                await _emailService.LogOrderPlaced(obj);
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnUserRegisterReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive the message
            var message = args.Message;

            var body = Encoding.UTF8.GetString(message.Body);

            string objMessage = JsonConvert.DeserializeObject<string>(body);

            try
            {
                //todo try to log email
                await _emailService.RegisterUserEmailAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive the message
            var message=args.Message;
            
            var body=Encoding.UTF8.GetString(message.Body);

            CartDto cartDto=JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                //todo try to log email
                await _emailService.EmailCartAndLog(cartDto);
                await args.CompleteMessageAsync(args.Message);

            }catch (Exception ex) {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            // if i want i can logg somewhere the error, or send an email 
            // console just for the test
            Console.WriteLine(args.Exception.ToString());   
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            await _serviceBusProcessor.StopProcessingAsync();
            await _serviceBusProcessor.DisposeAsync();


            // add this for the new queue about register user
            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();

            // add this for the email subscription
            await _emailOrderPlacedSubscription.StopProcessingAsync();
            await _emailOrderPlacedSubscription.DisposeAsync();
            
        }
    }
}
