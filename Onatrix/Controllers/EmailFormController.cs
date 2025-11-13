using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Onatrix.Models;
using Onatrix.Services;
using Onatrix.umbraco.models;
using System.Text.Json;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace Onatrix.Controllers
{
    public class EmailFormController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, FormSubmissionService formSubmissions, ServiceBusSender serviceBusSender) : SurfaceController(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        private readonly FormSubmissionService _formSubmissions = formSubmissions;
        private readonly ServiceBusSender _serviceBusSender = serviceBusSender;

        [HttpPost]
        public async Task<IActionResult> HandleEmailForm(EmailFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var result = _formSubmissions.SaveEmailRequest(model);
            if (!result)
            {
                TempData["EmailFormError"] = "Something went wrong while submitting your email address. Please try again later.";
                return RedirectToCurrentUmbracoPage();
            }

            try
            {
                var message = new ServiceBusMessage(JsonSerializer.Serialize(new MessageModel
                {
                    EmailAddress = model.Email
                }));
                await _serviceBusSender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to Service Bus: {ex.Message}");
            }
            TempData["EmailFormSuccess"] = "Thanks for reaching out - we will contact you within 24 hours!";

            return RedirectToCurrentUmbracoPage();
        }
    }

}
