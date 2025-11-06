using System.ComponentModel.DataAnnotations;

namespace Onatrix.umbraco.models;

public class QuestionFormViewModel
{
    [Required(ErrorMessage = "Name is required.")]
    [Display(Name = "Name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "You need to enter a question.")]
    [MinLength(10, ErrorMessage = "Your question must be at least 10 characters long.")]
    [Display(Name = "Question")]
    public string Question { get; set; } = null!;

    public string? PageUrl { get; set; } 
    public string? PageName { get; set; }
}
