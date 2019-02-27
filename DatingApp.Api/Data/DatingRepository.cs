using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data {
    public class DatingRepository : IDatingRepository {
        private readonly DataContext _db;

        public DatingRepository(DataContext db) {
            _db = db;
        }
        public void Add<T>(T entity) where T : class {
            _db.Add(entity); // this isn't async since we are only adding to the datacontext and NOT quering to the DB yet. This only happens in the RAM.
        }

        public void Delete<T>(T entity) where T : class {
            _db.Remove(entity);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _db.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id) {
            var user = await _db.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers() {
            var users = await _db.Users.Include(p => p.Photos).ToListAsync();
            return users;
        }

        public async Task<bool> SaveAll() {
            return await _db.SaveChangesAsync() > 0; // if changes are greater than 0 return true 
        }
    }
}