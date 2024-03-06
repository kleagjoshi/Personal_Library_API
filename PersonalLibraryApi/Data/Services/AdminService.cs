using Azure.Identity;
using PersonalLibraryApi.Data.Enum;
using PersonalLibraryApi.Data.Models;
using PersonalLibraryApi.Data.ViewModels;

namespace PersonalLibraryApi.Data.Services
{
    public class AdminService
    {
        private AppDbContext _context;
        public AdminService(AppDbContext context)
        {

            _context = context;

        }

        public List<UserVM> GetAllUsers()
        {
            List<UserVM> result = new List<UserVM>();

            var allUsers = _context.Users.ToList();

            var wishingStatus = StatusEnum.WishingList;
            var readingStatus = StatusEnum.Reading;

            foreach (var user in allUsers)
            {
                if (user.Email == "admin@admin.com") //skip the admin user
                {
                    allUsers.Remove(user);
                    break;
                }
            }

            foreach (var user in allUsers)
            {
                var _wishingListCount = _context.UserBooks.Where(n => n.UserId == user.Id && n.Status == wishingStatus).Count();

                var _readingCount = _context.UserBooks.Where(n => n.UserId == user.Id && n.Status == readingStatus).Count();

                var _record = new UserVM()
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    WishingListCount = _wishingListCount,
                    ReadingCount = _readingCount
                };
                result.Add(_record);
            }

            return result;
        }

        //delete user by id
        public bool DeleteUserById(string _userId)
        {
            var userbook = _context.UserBooks.FirstOrDefault(n => n.UserId == _userId);
            var user = _context.Users.FirstOrDefault(n => n.Id == _userId);

            if (userbook == null && user != null) //this user exists but has no books in his personal library
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }

            return false;
        }


        //get single user by id

        public UserVM GetUserById(string id)
        {

            var user = _context.Users.Where(n=>n.Id==id).FirstOrDefault();

            var wishingStatus = StatusEnum.WishingList;
            var readingStatus = StatusEnum.Reading;

            var _wishingListCount = _context.UserBooks.Where(n => n.UserId == user.Id && n.Status == wishingStatus).Count();

            var _readingCount = _context.UserBooks.Where(n => n.UserId == user.Id && n.Status == readingStatus).Count();

            var _record = new UserVM()
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                WishingListCount = _wishingListCount,
                ReadingCount = _readingCount
            };
                
            return _record;
        }



        //update user by id

        public ApplicationUser UpdateUserById(string userId, UpdateUserVM user)
        {
            //check if this user exist

            var _user = _context.Users.FirstOrDefault(n => n.Id == userId);
            if (_user != null)
            {
                _user.UserName = user.Username;
                _user.Email = user.Email;

                _context.SaveChanges();
            }

            return _user;

        }

        //get total number of users

        public int GetUsersCount ()
        {
            var count = _context.Users.Count() - 1; //-1 for admin
            return count;
        }

        //get total number of books

        public int GetBooksCount()
        {
            var count=_context.Books.Count();
            return count;
        }

        //get total number of authors

        public int GetAuthorsCount()
        {
            var count= _context.Authors.Count();
            return count;
        }

    }
}
