using System.ComponentModel.DataAnnotations;

namespace Onatrix.umbraco.models;

public class EmailFormViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address")]
    [Display(Name = "Email address")]
    public string Email { get; set; } = null!;
}
