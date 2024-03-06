using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalLibraryApi.Data.Enum;
using PersonalLibraryApi.Data.Services;
using PersonalLibraryApi.Data.ViewModels;

namespace PersonalLibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBooksController : ControllerBase
    {
        public UserBooksService _userBooksService;

        public UserBooksController(UserBooksService userBooksService)
        {
            _userBooksService = userBooksService;
        }

        //create the endpoint
        [HttpPost("add-book-user")] 
        
        public IActionResult AddBook([FromQuery]int bookId , [FromQuery] string userId)
        {
            _userBooksService.AddBookWithUser(bookId, userId);
            return Ok();
        }

        //create the endpoint
        [HttpGet("check-combination")]
        public IActionResult CheckCombination([FromQuery] int bookId, [FromQuery] string userId)
        {
            var userBook = _userBooksService.GetUserBookByCombination(bookId, userId);
            if (userBook != null)
            {
                return Ok(new { exists = true });
            }
            else
            {
                return Ok(new { exists = false });
            }
        }

        [HttpGet("get-all-books")]
        public IActionResult GetAllBooks([FromQuery] StatusEnum status, [FromQuery] string userId)
        {
            var allBooks = _userBooksService.GetAll(status,userId);
            return Ok(allBooks);
        }

        [HttpPut("update-user-book")]
        public IActionResult UpdateUserBook([FromQuery] int bookId, [FromQuery] string userId)
        {
             _userBooksService.UpdateBookWithUser(bookId, userId);
            return Ok();
        }
    }
}
