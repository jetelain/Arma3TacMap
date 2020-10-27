using Microsoft.AspNetCore.Authentication;

namespace Arma3TacMapWebApp.Models
{
    public class SignInViewModel
    {
        public string ReturnUrl { get; set; }
        public AuthenticationScheme[] Providers { get; set; }
    }
}
