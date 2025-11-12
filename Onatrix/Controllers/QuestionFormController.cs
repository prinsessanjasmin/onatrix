using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Onatrix.Models;
using Onatrix.Services;
using Onatrix.umbraco.models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using System.Text.Json;

namespace Onatrix.Controllers;

public class QuestionFormController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, FormSubmissionService formSubmissions, ServiceBusSender serviceBusSender)  : SurfaceController(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
{
    private readonly FormSubmissionService _formSubmissions = formSubmissions;
    private readonly ServiceBusSender _serviceBusSender = serviceBusSender;

    [HttpPost]
    public async Task<IActionResult> HandleQuestionForm(QuestionFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return CurrentUmbracoPage();
        }

        var currentPage = CurrentPage;
        var pageName = currentPage?.Name ?? "";
        var pageUrl = currentPage?.Url() ?? "";

        var result = _formSubmissions.SaveQuestionRequest(model, pageName, pageUrl);
        if (!result)
        {
            TempData["QuestionFormError"] = "Something went wrong while submitting your question. Please try again later.";
            return RedirectToCurrentUmbracoPage();
        }

        try
        {
            var message = new ServiceBusMessage(JsonSerializer.Serialize(new MessageModel
            {
                EmailAddress = model.Email,
                ClientName = model.Name,
                ServiceOfInterest = pageName
            }));

            await _serviceBusSender.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message to Service Bus: {ex.Message}");
        }


        TempData["QuestionFormSuccess"] = "Thanks for reaching out - we will contact you within 24 hours!";

        return RedirectToCurrentUmbracoPage();
    }
}
