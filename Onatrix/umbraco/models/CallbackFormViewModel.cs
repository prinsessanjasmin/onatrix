using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Onatrix.umbraco.models;

public class CallbackFormViewModel
{
    [Required(ErrorMessage = "Name is required.")]
    [Display(Name = "Name")]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Email is required.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address")]
    [Display(Name = "Email address")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "Phone is required.")]
    [Display(Name = "Phone")]
    public string Phone { get; set; } = null!;
    [Required(ErrorMessage = "You need to select an option")]
    public string SelectedService { get; set; } = null!;

    [BindNever]
    public IEnumerable<string> Options { get; set; } = [];
}


