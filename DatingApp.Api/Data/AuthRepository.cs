using System;
using System.Threading.Tasks;
using DatingApp.Api.Models;

namespace DatingApp.Api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _db;
        public AuthRepository(DataContext db)
        {
            _db = db;
        }
        public Task<User> Login(string username, string password)
        {
            byte[] passwordHash, passwordSalt;
            
            CreatePasswordHash(password, out passwordHash, out passwordSalt); // the out param is an necessary evil since we want to return two values.
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            var hmac = new System.Security.Cryptography.HMACSHA512();

        }

        public Task<User> Register(User user, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> UserExists(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}