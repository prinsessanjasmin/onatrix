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
            var container = _contentService.GetRootContent().FirstOrDefault(x => x.ContentType.Alias == "formSubmissions");
            if (container == null)
            {
                return false;
            }

            var requestName = $"{DateTime.Now:yyyy-MM-dd HH:mm} - {model.Name}";
            var request = _contentService.Create(requestName, container, "callbackRequest");

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

    public bool SaveQuestionRequest(QuestionFormViewModel model)
    {
        try
        {
            var container = _contentService.GetRootContent().FirstOrDefault(x => x.ContentType.Alias == "formSubmissions");
            if (container == null)
            {
                return false;
            }

            var requestName = $"{DateTime.Now:yyyy-MM-dd HH:mm} - {model.Name}";
            var request = _contentService.Create(requestName, container, "questionForm");

            request.SetValue("questionFormName", model.Name);
            request.SetValue("questionFormEmail", model.Email);
            request.SetValue("questionFormQuestion", model.Question);

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
            var container = _contentService.GetRootContent().FirstOrDefault(x => x.ContentType.Alias == "formSubmissions");
            if (container == null)
            {
                return false;
            }

            var requestName = $"{DateTime.Now:yyyy-MM-dd HH:mm} - {model.Email}";
            var request = _contentService.Create(requestName, container, "emailForm");

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
