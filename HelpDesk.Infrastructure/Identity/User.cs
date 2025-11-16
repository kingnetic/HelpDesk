using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Infrastructure.Identity
{
    public class User : IdentityUser<int>
    {
        [StringLength(200)]
        public string? FullName { get; set; }
    }
}
