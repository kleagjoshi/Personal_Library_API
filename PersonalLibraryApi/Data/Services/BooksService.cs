
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalLibraryApi.Data.Models;
using PersonalLibraryApi.Data.ViewModels;
using System.Net;

namespace PersonalLibraryApi.Data.Services
{
    public class BooksService
    {
        //communicates with db

        private readonly AppDbContext _context;
        public BooksService(AppDbContext context)
        {

            _context = context;

        }

        //add a book
        public async Task<Book> AddBookWithAuthors(BookVM book)
        {
            var _book = new Book()
            {
                Title = book.Title,
                Description = book.Description,
                CoverUrl = book.CoverUrl,
                GenreId = book.GenreId

            };
             await _context.Books.AddAsync(_book);
             await _context.SaveChangesAsync();

            //add relations of book and book author to bookauthor tables

            foreach (var id in book.AuthorIds)
            {
                var _book_author = new Book_Author()
                {
                    BookId = _book.BookId,
                    AuthorId = id
                };

                await _context.Books_Authors.AddAsync(_book_author);
                await _context.SaveChangesAsync();
            }

            return _book;
        }
        //combination check if already exists
        public Book GetBookByAuthorTitleCombination(int authorId, string title)
        {
            return _context.Books
                .Include(b => b.Book_Authors)
                .FirstOrDefault(b => b.Book_Authors.Any(a => a.AuthorId == authorId) && b.Title.ToLower() == title.ToLower());
        }

        //get list of books
        public List<BookWithAuthorsVM> GetAllBooks()
        {
            List<BookWithAuthorsVM> result = new List<BookWithAuthorsVM>();
            var allbooks = _context.Books.ToList();
            //return allbooks;

            foreach (var eachbook in allbooks)
            {
                var _bookWithAuthors = _context.Books.Where(n => n.BookId == eachbook.BookId).Select(book => new BookWithAuthorsVM()
                {
                    BookId = eachbook.BookId,
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

        //get a single book by id

        //public Book GetBookById(int bookId) => _context.Books.FirstOrDefault(n => n.Id == bookId);

        public BookWithAuthorsVM GetBookById(int bookId)
        {
            var _bookWithAuthors = _context.Books.Where(n => n.BookId == bookId).Select(book => new BookWithAuthorsVM()
            {
                BookId=bookId,
                Title = book.Title,
                Description = book.Description,
                CoverUrl = book.CoverUrl,
                GenreTitle = book.Genre.GenreTitle,
                AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
            }).FirstOrDefault();

            return _bookWithAuthors;
        }


        //update existing data
        public bool UpdateBookById(int bookId, BookWithAuthorsVM book)
        {
            //check if this book exist
            var _book = _context.Books.FirstOrDefault(n => n.BookId == bookId);
            if (_book != null)
            {
                _book.Title = book.Title;
                _book.Description = book.Description;
                _book.CoverUrl = book.CoverUrl;
                _book.GenreId = _context.Genres.FirstOrDefault(g => g.GenreTitle == book.GenreTitle).GenreId;

                // Remove existing relations
                var existingRelations = _context.Books_Authors.Where(ba => ba.BookId == bookId).ToList();
                _context.Books_Authors.RemoveRange(existingRelations);

                // Update existing authors
                foreach (var authorName in book.AuthorNames)
                {
                    var author = _context.Authors.FirstOrDefault(a => a.FullName == authorName);
                    if (author != null)
                    {
                        var updatedAuthor = _context.Authors.FirstOrDefault(a => a.AuthorId == author.AuthorId);
                        if (updatedAuthor != null)
                        {
                            updatedAuthor.FullName = author.FullName; // Update the FullName property of the Author object
                            _context.Authors.Update(updatedAuthor);
                        }
                    }
                }

                // Add new relations
                foreach (var authorName in book.AuthorNames)
                {
                    var author = _context.Authors.FirstOrDefault(a => a.FullName == authorName);
                    if (author != null)
                    {
                        var _book_author = new Book_Author()
                        {
                            BookId = _book.BookId,
                            AuthorId = author.AuthorId
                        };

                        _context.Books_Authors.Add(_book_author);
                    }
                }

                _context.SaveChanges();

                return true;
            }

            return false;
        }

        //delete book by id

        public bool DeleteBookById(int bookId)
        {
            //check if the book exists
            var book = _context.Books.FirstOrDefault(n => n.BookId == bookId);

            var findBook = _context.UserBooks.FirstOrDefault(n => n.BookId == bookId);

            if (book != null && findBook == null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }


            //check if none of the users has this book in his personal library(profile)
        }
        //add service for searching a book
        public List<Book> SearchBooksByTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                // Handle empty search term
                return new List<Book>();
            }

            return _context.Books
                .Where(b => EF.Functions.Like(b.Title, $"%{title}%"))
                .ToList();
        }
    }
}

