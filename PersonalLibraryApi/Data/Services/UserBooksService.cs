using PersonalLibraryApi.Data.Enum;
using PersonalLibraryApi.Data.Models;
using PersonalLibraryApi.Data.ViewModels;

namespace PersonalLibraryApi.Data.Services
{
    public class UserBooksService
    {
        private AppDbContext _context;
        public UserBooksService(AppDbContext context)
        {

            _context = context;

        }

        //add a new book-user to db
        public void AddBookWithUser(int bookId,string userId)
        {
            var _bookUser = new UserBooks()
            {
                BookId=bookId,
                UserId=userId,
                Status=0
            };
            _context.UserBooks.Add(_bookUser);
            _context.SaveChanges();

        }

        //combination check if already exists
        public UserBooks GetUserBookByCombination(int bookId, string userId)
        {
            return _context.UserBooks.FirstOrDefault(ub => ub.BookId == bookId && ub.UserId == userId);
        }


        //get list of books by status
        public List<BookWithAuthorsVM> GetAll(StatusEnum status, string userID)
        {
            List<BookWithAuthorsVM> result = new List<BookWithAuthorsVM>();
            List<int> BookIDs = new List<int>();

            var allRecords = _context.UserBooks.ToList();

            foreach (var record in allRecords) {
            
                if(record.Status.Equals(status) && record.UserId==userID)
                {
                    BookIDs.Add(record.BookId);
                }
            }

            foreach (var eachBookId in BookIDs)
            {
                var _bookWithAuthors = _context.Books.Where(n => n.BookId == eachBookId).Select(book => new BookWithAuthorsVM()
                {
                    BookId = eachBookId,
                    Title = book.Title,
                    Description = book.Description,
                    CoverUrl = book.CoverUrl,
                    GenreTitle = book.Genre.GenreTitle,
                    AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
                }).FirstOrDefault();

                result.Add(_bookWithAuthors);
            }

            return result;
        }

        //update user book record

        public void UpdateBookWithUser(int bookId, string userId)
        {
            var _record = _context.UserBooks.FirstOrDefault(n => n.BookId == bookId && n.UserId==userId);
            if (_record != null)
            {
                _record.Status = (Enum.StatusEnum)1;
                _context.SaveChanges();
            }

        }

    }
}
