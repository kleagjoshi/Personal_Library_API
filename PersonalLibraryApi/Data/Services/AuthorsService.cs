using PersonalLibraryApi.Data.Models;
using PersonalLibraryApi.Data.ViewModels;

namespace PersonalLibraryApi.Data.Services
{
    public class AuthorsService
    {
            private readonly AppDbContext _context;
            public AuthorsService(AppDbContext context)
            {

                _context = context;

            }

            //add an author to db

            public Author AddAuthor(AuthorVM author)
            {
                var _author = new Author()
                {
                    FullName = author.FullName
                };

             _context.Authors.Add(_author);
             _context.SaveChanges();
            return _author;
            }

            //get authors with books
            public AuthorWithBookVM GetAuthorWithBooks(int authorId)
            {
                var _author = _context.Authors.Where(n => n.AuthorId == authorId).Select(n => new AuthorWithBookVM()
                {
                    FullName = n.FullName,
                    BookTitles = n.Book_Authors.Select(n => n.Book.Title).ToList()

                }).FirstOrDefault();

                return _author;
            }

        //get all authors
        public List<Author> GetAllAuthors()
        {
            var allauthors = _context.Authors.ToList();
            return allauthors;
        }
    }
    }


