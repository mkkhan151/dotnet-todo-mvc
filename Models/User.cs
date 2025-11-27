using Microsoft.AspNetCore.Identity;

namespace TodoApp.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = null!;
    }
}