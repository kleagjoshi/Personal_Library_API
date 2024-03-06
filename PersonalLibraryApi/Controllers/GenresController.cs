using Microsoft.AspNetCore.Mvc;
using PersonalLibraryApi.Data.Services;
using PersonalLibraryApi.Data.ViewModels;

namespace PersonalLibraryApi.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        
        public class GenresController : ControllerBase
        {
            GenresService _genresService;
            public GenresController(GenresService genresService)
            {
                _genresService = genresService;
            }

            //Get all genres
            [HttpGet("get-all-genres")]
            public IActionResult GetAllGenres()
            {
                try
                {
                    var _result = _genresService.GetAllGenres();
                    return Ok(_result);
                }
                catch (Exception)
                {
                    return BadRequest("We could not load the genres");

                }
            }

            //Get all publishers - used for sorting,filtering and paging as well
            /*[HttpGet("get-all-publishers")]
            public IActionResult GetAllPublishers(string sortBy,string searchString)
            {
                try
                {
                    var _result = _publishersService.GetAllPublishers(sortBy,searchString);
                    return Ok(_result);
                }
                catch (Exception )
                {
                    return BadRequest("We could not load the publishers");

                }
            }*/
            [HttpPost("add-genre")]
            public IActionResult AddGenre([FromBody] GenreVM genre)
            {
                try
                {
                    var newGenre = _genresService.AddGenre(genre);
                    return Ok(newGenre);

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);

                }

            }

           /* [HttpGet("get-publisher-books-with-author/{id}")]
            public IActionResult GetPublisherData(int id)
            {
                var _response = _publishersService.GetPublisherData(id);
                return Ok(_response);
            }*/

            [HttpGet("get-genre-by-id/{id}")]
            public IActionResult GetGenreById(int id)
            {

                var _response = _genresService.GetGenreById(id);
                if (_response != null)
                {
                    return Ok(_response);

                }
                else
                {
                    return NotFound();
                }

            }

            [HttpDelete("delete-genre-by-id/{id}")]
            public IActionResult DeleteGenreById(int id)
            {
                try
                {
                    _genresService.DeleteGenreById(id);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);

                }
            }

        }
    }

