using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalLibraryApi.Data.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverUrl { get; set; }

        //list of author and list of genre
        //Navigation properties   
        public int GenreId { get; set; } //foreign key
        public Genre Genre { get; set; }

        //many to many - authors
        public List<Book_Author> Book_Authors { get; set; }

        //many to many - users

        public List<UserBooks> UserBooks { get; set; }
    }
}
