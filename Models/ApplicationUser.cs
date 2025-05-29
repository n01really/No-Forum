using Microsoft.AspNetCore.Identity;

namespace No_Forum.Models
{
    
    
        public class ApplicationUser : IdentityUser
        {
            public string? ProfilePicturePath { get; set; }
           
        }
}
