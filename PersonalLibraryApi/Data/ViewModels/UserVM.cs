namespace PersonalLibraryApi.Data.ViewModels
{
    public class UserVM
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int WishingListCount { get; set; }
        public int ReadingCount { get; set; }
    }
}
