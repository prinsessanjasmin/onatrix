using Onatrix.umbraco.models;
using Umbraco.Cms.Core.Services;

namespace Onatrix.Services;

public class FormSubmissionService(IContentService contentService)
{
    private readonly IContentService _contentService = contentService;

    public bool SaveCallbackRequest(CallbackFormViewModel model)
    {
        try
        {
            var root = _contentService.GetRootContent();
            var callbackSubmissions = root.FirstOrDefault(x => x.Name == "Callback Submissions");
            
            if (callbackSubmissions == null)
            {
                return false;
            }

            var requestName = $"{DateTime.Now:yyyy-MM-dd HH:mm} - {model.Name}";
            var request = _contentService.Create(requestName, callbackSubmissions, "callbackRequest");

            request.SetValue("callbackRequestName", model.Name);
            request.SetValue("callbackRequestEmail", model.Email);
            request.SetValue("callbackRequestPhone", model.Phone);
            request.SetValue("callbackRequestSelectedService", model.SelectedService);

            var saveResult = _contentService.Save(request);
            return saveResult.Success;
        }
        catch (Exception ex) 
        {
            return false; 
        }
        
    }

    public bool SaveQuestionRequest(QuestionFormViewModel model, string pageTitle, string pageUrl)
    {
        try
        {
            var root = _contentService.GetRootContent();
            var questionSubmissions = root.FirstOrDefault(x => x.Name == "Question Submissions");

            if (questionSubmissions == null)
            {
                return false;
            }

            var requestName = $"{DateTime.Now:yyyy-MM-dd HH:mm} - {model.Name}";
            var request = _contentService.Create(requestName, questionSubmissions, "questionForm");

            request.SetValue("questionFormName", model.Name);
            request.SetValue("questionFormEmail", model.Email);
            request.SetValue("questionFormQuestion", model.Question);
            request.SetValue("questionFormOriginPageTitle", pageTitle);
            request.SetValue("questionFormOriginPageUrl", pageUrl);

            var saveResult = _contentService.Save(request);
            return saveResult.Success;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public bool SaveEmailRequest(EmailFormViewModel model)
    {
        try
        {
            var root = _contentService.GetRootContent();
            var emailSubmissions = root.FirstOrDefault(x => x.Name == "Email Submissions");

            if (emailSubmissions == null)
            {
                return false;
            }

            var requestName = $"{DateTime.Now:yyyy-MM-dd HH:mm} - {model.Email}";
            var request = _contentService.Create(requestName, emailSubmissions, "emailForm");

            request.SetValue("emailFormEmail", model.Email);

            var saveResult = _contentService.Save(request);
            return saveResult.Success;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
