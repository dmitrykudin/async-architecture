using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace AsyncArchitecture.Identity.ViewModels.Account
{
    public class RegisterViewModel : RegisterInputModel
    {
        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}