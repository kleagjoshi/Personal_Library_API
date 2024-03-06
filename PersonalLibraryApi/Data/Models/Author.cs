using System.ComponentModel.DataAnnotations;

namespace PersonalLibraryApi.Data.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        public string FullName { get; set; }

        //Navigation properties

        public List<Book_Author> Book_Authors { get; set; } //many to many
    }
}
