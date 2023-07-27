namespace Mango.MessageBus
{
    public interface IMessageBus
    {
        //i cannot have the same name in queue and topics
        Task PublishMessage(object message,string topic_queue_Name);
    }
}
