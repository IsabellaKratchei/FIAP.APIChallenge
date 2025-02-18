namespace FIAP.APIRegiao.Events
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = "localhost";
        //public string HostName { get; set; } = "host.docker.internal";
        public string QueueName { get; set; } = "RegiaoQueue";
    }
}
