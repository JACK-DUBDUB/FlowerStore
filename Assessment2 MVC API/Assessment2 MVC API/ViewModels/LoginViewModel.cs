using System.ComponentModel.DataAnnotations;

// Information from https://youtu.be/T0ZnrENlOfw?list=PL82C6-O4XrHde_urqhKJHH-HTUfTK6siO



namespace Assessment2_MVC_API.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address is required.")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
