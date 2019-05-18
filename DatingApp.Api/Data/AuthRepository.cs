using System;
using System.Threading.Tasks;
using DatingApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data {
    public class AuthRepository : IAuthRepository {
        private readonly DataContext db;
        public AuthRepository(DataContext db) {
            this.db = db;
        }
        public async Task<User> Login(string username, string password) {
            var user = await this.db.Users.Include(u => u.Photos).FirstOrDefaultAsync(x => x.Username == username);
            if (user == null) {
                return null;
            }
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) {
                return null;
            }
            return user;

        }

        public async Task<User> Register(User user, string password) {

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt); // the out param is an necessary evil since we want to return two values. 

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await this.db.Users.AddAsync(user);
            await this.db.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                // if (computedHash == passwordHash) // TODO: Test and see if we can compare byte[] directly
                //     return false;

                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != passwordHash[i]) {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<bool> UserExists(string username) {
            if (await this.db.Users.AnyAsync(x => x.Username == username)) {
                return true;
            }
            return false;
        }
    }
}