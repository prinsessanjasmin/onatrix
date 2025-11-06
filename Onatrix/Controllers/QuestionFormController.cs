using Microsoft.AspNetCore.Mvc;
using Onatrix.Services;
using Onatrix.umbraco.models;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace Onatrix.Controllers;

public class QuestionFormController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, FormSubmissionService formSubmissions)  : SurfaceController(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
{
    private readonly FormSubmissionService _formSubmissions = formSubmissions;

    [HttpPost]
    public IActionResult HandleQuestionForm(QuestionFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return CurrentUmbracoPage();
        }

        var result = _formSubmissions.SaveQuestionRequest(model);
        if (!result)
        {
            TempData["QuestionFormError"] = "Something went wrong while submitting your question. Please try again later.";
            return RedirectToCurrentUmbracoPage();
        }

        TempData["QuestionFormSuccess"] = "Thanks for reaching out - we will contact you within 24 hours!";

        return RedirectToCurrentUmbracoPage();
    }
}
