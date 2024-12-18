namespace FIAP.APIRegiao.Events
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = "localhost";
        public string QueueName { get; set; } = "RegiaoQueue";
    }
}
