namespace PersonalLibraryApi.Data.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string GenreTitle { get; set; }

        //Navigation properties
        public List<Book> Books { get; set; }
    }
}
