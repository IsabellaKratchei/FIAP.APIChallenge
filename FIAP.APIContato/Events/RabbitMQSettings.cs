﻿namespace FIAP.APIContato.Events
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } ="localhost";
        public string UserName { get; set; }= "guest";
        public string Password { get; set; } = "guest";
        public string QueueName { get; set; } = "RegiaoQueue";
    }
}
