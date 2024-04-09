using SiteManagement.Application.Messaging;
using SiteManagement.Application.Services.Messages;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Events.Messages;
using SiteManagement.Projections.SendMessageService.Services;

namespace SiteManagement.Projections.SendMessageService
{
    public class Worker : BackgroundService
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMessageService _messageService;

        public Worker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _messageService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMessageService>();
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {


                QueueFactory.CreateBasicConsumer()
                    .EnsureExchange(exchangeName: "MessageExchange")
                    .EnsureQueue(queueName: "SendMessageQueue", exchangeType: "MessageExchange")
                    .Receive<SendMessageEvent>(@event =>
                    {
                        Message message = new Message()
                        {
                            Text = @event.Message,
                            ReceiverId = @event.ReceiverId,
                            SenderId = @event.SenderId,
                        };
                        _messageService.SendMessage(message, stoppingToken).GetAwaiter().GetResult();
                    })
                    .StartConsuming("SendMessageQueue");

            
            return Task.CompletedTask;

            
        }
    }
}
