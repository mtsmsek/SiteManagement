using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace SiteManagement.Application.Messaging;

public static class QueueFactory
{
    public static void SendMessageToExchange(string exchangeName,
                                             string exchangeType,
                                             string queueName,
                                             object obj)
    {
        var channel = CreateBasicConsumer()
                      .EnsureExchange(exchangeName: exchangeName, exchangeType: exchangeType)
                      .EnsureQueue(queueName: queueName, exchangeType: exchangeName)
                      .Model;

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));

        channel.BasicPublish(exchange: exchangeName,
                             routingKey: queueName,
                             basicProperties: null,
                             body:body);
    }

    public static EventingBasicConsumer CreateBasicConsumer()
    {
        var factory = new ConnectionFactory()
        {
            //todo -- move it to constant classs
            HostName = "localhost",

        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        return new EventingBasicConsumer(channel);
    }
    //todo -- move direct to constant class
    public static EventingBasicConsumer EnsureExchange(this EventingBasicConsumer consumer,
                                                       string exchangeName,
                                                       string exchangeType = "direct")
    {
        consumer.Model.ExchangeDeclare(exchange: exchangeName,
                                        type: exchangeType,
                                        durable: false,
                                        autoDelete: false);
        return consumer;
           
                                        
    }
    public static EventingBasicConsumer EnsureQueue(this EventingBasicConsumer consumer,
                                                     string queueName,
                                                     string exchangeType)
    {
        consumer.Model.QueueDeclare(queue:queueName,
                                                durable: false,
                                                exclusive:false,
                                                autoDelete: false,
                                                arguments: null);

        consumer.Model.QueueBind(queueName, exchangeType, queueName);

        return consumer;
    }

    public static EventingBasicConsumer Receive<T>(this EventingBasicConsumer consumer, Action<T> action)
    {
        consumer.Received += (m, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var model = JsonSerializer.Deserialize<T>(message);

            action(model);

            consumer.Model.BasicAck(eventArgs.DeliveryTag, false);
        };
        return consumer;
    }

    public static EventingBasicConsumer StartConsuming(this EventingBasicConsumer consumer, string queueName)
    {
        consumer.Model.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
        return consumer;
    }
}
