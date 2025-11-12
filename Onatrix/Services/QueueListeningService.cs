using Azure.Messaging.ServiceBus;
using Onatrix.Models;

namespace Onatrix.Services;

public class QueueListeningService(ServiceBusProcessor serviceBusProcessor, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ServiceBusProcessor _serviceBusProcessor = serviceBusProcessor;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    //Had help here from Claude AI, who suggested the use of IServiceProvider instead of injecting the EmailClient directly

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _serviceBusProcessor.ProcessMessageAsync += MessageHandler;
        _serviceBusProcessor.ProcessErrorAsync += ErrorHandler;

        await _serviceBusProcessor.StartProcessingAsync(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    
    public async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var message = args.Message.Body.ToString();
        var messageData = System.Text.Json.JsonSerializer.Deserialize<MessageModel>(message);

        if (messageData != null)
        {
            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

            await emailService.SendEmailAsync(messageData.EmailAddress, messageData.ClientName ?? "", messageData.ServiceOfInterest ?? "");
        }

        await args.CompleteMessageAsync(args.Message);       
    }

    // handle any errors when receiving messages
    public Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}
