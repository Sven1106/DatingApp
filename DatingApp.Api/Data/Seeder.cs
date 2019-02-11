using System.Collections.Generic;
using DatingApp.Api.Models;
using Newtonsoft.Json;

namespace DatingApp.Api.Data {
    public class Seed {
        public DataContext _db { get; }
        public Seed(DataContext db) {
            _db = db;
        }

        public void SeedUsers() {
            var userSeedData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(userSeedData);
            foreach (var user in users) {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("Password", out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();

                _db.Users.Add(user);
            }
            _db.SaveChanges();
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}