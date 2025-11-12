using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Azure.Communication.Email;
using Onatrix.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<FormSubmissionService>();
builder.Services.AddScoped<EmailService>();

//Got help with service bus registration from Claude AI 
builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration["ServiceBus:ConnectionString"];
    return new ServiceBusClient(connectionString);
});

builder.Services.AddSingleton(sp =>
{
    var serviceBusClient = sp.GetRequiredService<ServiceBusClient>();
    var queueName = builder.Configuration["ServiceBus:QueueName"];
    return serviceBusClient.CreateSender(queueName);
});

builder.Services.AddSingleton(sp =>
{
    var serviceBusProcessor = sp.GetRequiredService<ServiceBusClient>();
    var queueName = builder.Configuration["ServiceBus:QueueName"];
    return serviceBusProcessor.CreateProcessor(queueName, new ServiceBusProcessorOptions());
});

builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("ACS:ConnectionString");
    return new EmailClient(connectionString);
});


builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();

await app.BootUmbracoAsync();

app.UseHttpsRedirection();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
