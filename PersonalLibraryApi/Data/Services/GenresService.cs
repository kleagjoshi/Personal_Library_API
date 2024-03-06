using PersonalLibraryApi.Data.Models;
using PersonalLibraryApi.Data.ViewModels;
using System.Text.RegularExpressions;

namespace PersonalLibraryApi.Data.Services
{
    public class GenresService
    {
            AppDbContext _context;
            public GenresService(AppDbContext context)
            {
                _context = context;
            }

            public List<Genre> GetAllGenres()
            {
                var allGenres = _context.Genres.ToList();


                return allGenres;
            }

            
            public Genre AddGenre(GenreVM genre)
            {

                var _genre = new Genre()
                {
                    GenreTitle = genre.GenreTitle
                };

                _context.Genres.Add(_genre); //add to db
                _context.SaveChanges();
                return _genre;
            }

            public Genre GetGenreById(int id)
            {
                return _context.Genres.FirstOrDefault(n => n.GenreId == id);
            }
            

            //search by genre name
            /*
            public PublisherWithBooksAndAuthorsVM GetPublisherData(int publisherId)
            {
                var _publisherData = _context.Publishers.Where(n => n.Id == publisherId).Select(n => new PublisherWithBooksAndAuthorsVM()
                {

                    Name = n.Name,
                    BookAuthors = n.Books.Select(n => new BookAuthorVM()
                    {
                        BookName = n.Title,
                        BookAuthors = n.Book_Authors.Select(n => n.Author.FullName).ToList()

                    }).ToList()

                }).FirstOrDefault();

                return _publisherData;
            }*/

            public void DeleteGenreById(int id)
            {
                var _genre = _context.Genres.FirstOrDefault(n => n.GenreId == id);
                if (_genre != null)
                {
                    _context.Genres.Remove(_genre);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception($"The genre with id {id} does not exist");
                }
            }

           
        }
    }

