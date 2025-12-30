using Microsoft.AspNetCore.Identity;

namespace sh2.Models
{
    // Наследуем от IdentityRole<int> для поддержки ASP.NET Core Identity
    public class Role : IdentityRole<int>
    {
    }
}