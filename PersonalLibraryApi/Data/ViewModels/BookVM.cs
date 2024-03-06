namespace PersonalLibraryApi.Data.ViewModels
{
    public class BookVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverUrl { get; set; }
        public int GenreId { get; set; }
        public List<int> AuthorIds { get; set; }
    }
    public class BookWithAuthorsVM
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }  
        public string CoverUrl { get; set; }
        public string GenreTitle { get; set; }
        public List<string> AuthorNames { get; set; }
    }
}
