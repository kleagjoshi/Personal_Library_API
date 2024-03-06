using Microsoft.EntityFrameworkCore;
using PersonalLibraryApi.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace PersonalLibraryApi.Data.Models
{
    public class UserBooks
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Book Book { get; set; }
        public int BookId { get; set; }
        public StatusEnum Status { get; set; }




    }
}
