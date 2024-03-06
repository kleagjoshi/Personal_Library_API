using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalLibraryApi.Data;
using PersonalLibraryApi.Data.Services;
using PersonalLibraryApi.Data.ViewModels;

namespace PersonalLibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        public BooksService _booksService;

            public BooksController(BooksService booksService)
            {
                _booksService = booksService;
            }

        //create the endpoint
        //[Authorize(Roles= UserRoles.Admin)]
        [HttpPost("add-book-with-authors")]
            public async Task<IActionResult> AddBook([FromBody] BookVM book)
            {
                await _booksService.AddBookWithAuthors(book);
                return Ok();
            }

            //check if already exists in library
            [HttpGet("check-author-title")]
            public IActionResult CheckAuthorTitle([FromQuery] int authorId, [FromQuery] string title)
            {
                var book = _booksService.GetBookByAuthorTitleCombination(authorId, title);
                if (book != null)
                {
                    return Ok(new { exists = true });
                }
                else
                {
                    return Ok(new { exists = false });
                }
            }
        
        //update book by id
        //[Authorize(Roles= UserRoles.Admin)]
        [HttpPut("update-book-by-id/{id}")]
            public IActionResult UpdateBookById(int id, [FromBody] BookWithAuthorsVM bookVM)
            {
                var result = _booksService.UpdateBookById(id, bookVM);
                return Ok(result);
            }


            //get all books endpoint
            [HttpGet("get-all-books")]
            public IActionResult GetAllBooks()
            {
                var allBooks = _booksService.GetAllBooks();
                return Ok(allBooks);
            }

            [HttpGet("get-book-by-id/{id}")] //{ } what we get from the http request , it has to match with the parameter below
            public IActionResult GetBookById(int id)
            {
                var book = _booksService.GetBookById(id);
                return Ok(book);
            }

        //delete a book by id
        //[Authorize(Roles= UserRoles.Admin)]
        [HttpDelete("delete-a-book-by-id/{id}")]
            public IActionResult DeleteBookById(int id)
            {
                var result = _booksService.DeleteBookById(id);
                return Ok(result);
            }
        
        //search book by tittle
        //[HttpPost("search-book-by-title/{title}")]
        //public IActionResult Search(string title)
        //{
        //    var books = _booksService.SearchBooksByTitle(title);
        //    return Ok(books);
        //}

        [HttpGet("search-book-by-title/{title}")]
        public IActionResult Search(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return NotFound();
            }

            var books = _booksService.SearchBooksByTitle(title);
            return Ok(books);
        }

    }
    }

