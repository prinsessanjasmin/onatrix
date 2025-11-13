using Azure.Messaging.ServiceBus;
using Onatrix.Models;

namespace Onatrix.Services;

public class QueueListeningService : BackgroundService
{
    private readonly ServiceBusProcessor _serviceBusProcessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueueListeningService> _logger;

    public QueueListeningService(ServiceBusProcessor serviceBusProcessor, IServiceProvider serviceProvider, ILogger<QueueListeningService> logger)
    {
        _serviceBusProcessor = serviceBusProcessor;
        _serviceProvider = serviceProvider;
        _logger = logger;

        _logger.LogInformation("QueueListeningService constructor called");
    }


    //Had help here from Claude AI, who suggested the use of IServiceProvider instead of injecting the EmailClient directly, and suggested extra logging 

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("QueueListeningService starting...");

        _serviceBusProcessor.ProcessMessageAsync += MessageHandler;
        _serviceBusProcessor.ProcessErrorAsync += ErrorHandler;

        await _serviceBusProcessor.StartProcessingAsync(stoppingToken);

        _logger.LogInformation("ServiceBusProcessor started successfully");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    
    public async Task MessageHandler(ProcessMessageEventArgs args)
    {

        _logger.LogInformation("Message received from queue!");
        try
        {
            var message = args.Message.Body.ToString();
            _logger.LogInformation($"Message body: {message}");

            var messageData = System.Text.Json.JsonSerializer.Deserialize<MessageModel>(message);

            if (messageData != null)
            {
                _logger.LogInformation($"Deserialized message for: {messageData.EmailAddress}");

                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                _logger.LogInformation("About to send email...");
                await emailService.SendEmailAsync(messageData.EmailAddress, messageData.ClientName ?? "", messageData.ServiceOfInterest ?? "");
                _logger.LogInformation("Email sent successfully!");

                await args.CompleteMessageAsync(args.Message);
                _logger.LogInformation("Message completed");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
        }
       
        
    }

    // handle any errors when receiving messages
    public Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, $"Error in Service Bus: {args.ErrorSource}");

        return Task.CompletedTask;
    }
}
