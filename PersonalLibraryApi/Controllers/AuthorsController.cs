using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalLibraryApi.Data.Services;
using PersonalLibraryApi.Data.ViewModels;
using PersonalLibraryApi.Data.ViewModels.Authentication;

namespace PersonalLibraryApi.Controllers
{
    public class AuthorsController :ControllerBase
    {
        private AuthorsService _authorsService;

        public AuthorsController(AuthorsService authorsService)
        {
            _authorsService = authorsService;
        }

        //[Authorize(Roles= UserRoles.Admin)]
        [HttpPost("add-author")]
        public IActionResult AddAuthor([FromBody] AuthorVM author)
        {
            var newAuthor =  _authorsService.AddAuthor(author);
            return Ok(newAuthor);

        }

        [HttpGet("get-author-with-books/{id}")]
        public IActionResult GetAuthorWithBooks(int id)
        {
            var response = _authorsService.GetAuthorWithBooks(id);
            return Ok(response);
        }

        [HttpGet("get-all-authors")]
        public IActionResult GetAllAuthors()
        {
            var allAuthors = _authorsService.GetAllAuthors();
            return Ok(allAuthors);
        }

    }
}
