using Microsoft.AspNetCore.Identity;

namespace PersonalLibraryApi.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        //many to many - books

        public List<UserBooks> UserBooks { get; set; }
    }
}
