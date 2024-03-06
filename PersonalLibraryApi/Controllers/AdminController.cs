using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalLibraryApi.Data.Services;
using PersonalLibraryApi.Data.ViewModels;

namespace PersonalLibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        public AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("get-all-users")]
        public IActionResult GetAllUsers()
        {
            var allUsers = _adminService.GetAllUsers();
            return Ok(allUsers);
        }

        //delete a user by id
        [HttpDelete("delete-a-user-by-id")]
        public IActionResult DeleteUserById([FromQuery] string id)
        {
            var result = _adminService.DeleteUserById(id);
            return Ok(result);
        }

        //get user by id
        [HttpGet("get-user-by-id")] //{ } what we get from the http request , it has to match with the parameter below
        public IActionResult GetBookById([FromQuery] string id)
        {
            var user = _adminService.GetUserById(id);
            return Ok(user);
        }


        //update user by id
        [HttpPut("update-user-by-id")]
        public IActionResult UpdateBookById([FromQuery] string id, [FromBody] UpdateUserVM userVM)
        {
            var updatedUser = _adminService.UpdateUserById(id, userVM);
            return Ok(updatedUser);
        }

        //get users count
        [HttpGet("get-users-count")]
        public IActionResult GetUsersCount()
        {
            var allUsers = _adminService.GetUsersCount();
            return Ok(allUsers);
        }

        //get books count
        [HttpGet("get-books-count")]
        public IActionResult GetBooksCount()
        {
            var allBooks = _adminService.GetBooksCount();
            return Ok(allBooks);
        }

        //get authors count
        [HttpGet("get-authors-count")]
        public IActionResult GetAuthorsCount()
        {
            var allAuthors = _adminService.GetAuthorsCount();
            return Ok(allAuthors);
        }

    }

    
}
