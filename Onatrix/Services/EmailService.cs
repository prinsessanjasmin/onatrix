using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;

namespace Onatrix.Services;

public class EmailService
{
    private readonly EmailClient _emailClient;
    private readonly IConfiguration _configuration;

    public EmailService(EmailClient emailClient, IConfiguration configuration)
    {
        _emailClient = emailClient;
        _configuration = configuration;
    }
    public async Task SendEmailAsync(string emailAddress, string name, string serviceOfInterest)
    {
        var emailContent = new EmailContent("");
        if (string.IsNullOrWhiteSpace(serviceOfInterest))
        {
            emailContent = new EmailContent("How can we be of service?")
            {
                PlainText = "Hello, \n\nThank you for reaching out to us at Onatrix! Can you tell us more about what we can do for you, so our best suited proffessional can get in touch with you?\n\nBest regards,\nOnatrix Team",
                Html = @"
                    <html>
                        <body>
                            <h2>Hello!</h2>
                            <p>Thank you for reaching out to us at Onatrix! Can you tell us more about what we can do for you, so our best suited proffessional can get in touch with you?<br><br>Best regards,<br>Onatrix Team</p>
                        </body>
                    </html>"
            };
        }
        else
        {
            emailContent = new EmailContent($"Your interest in {serviceOfInterest}")
            {
                PlainText = $"Hello {name ?? "to you"},\n\nThank you for reaching out to us at Onatrix! We're looking forward to dive deeper into {serviceOfInterest} with you. We will get back to you shortly.\n\nBest regards,\nOnatrix Team",
                Html = $@"
                <html>
                    <body>
                        <h2>Hello!</h2>
                        <p>Thank you for reaching out to us at Onatrix! We're looking forward to dive deeper into {serviceOfInterest} with you. We will get back to you shortly.<br><br>Best regards,<br>Onatrix Team</p>
                    </body>
                </html>"
            };
        }
        
        var emailMessage = new EmailMessage(
            senderAddress: _configuration["SenderAddress"],
            content: emailContent,
            recipients: new EmailRecipients(new List<EmailAddress>
            {
                new(emailAddress)
            }));


        var emailSendOperation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
    }
}
