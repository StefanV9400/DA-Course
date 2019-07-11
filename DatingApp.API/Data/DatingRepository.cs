using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T enitty) where T : class
        {
            _context.Remove(enitty);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain); 
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos
                .FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users
                .OrderByDescending(u => u.LastActive)
                .Include(u => u.Photos)
                .AsQueryable();;
            
            users = users
                .Where(x => x.Id != userParams.UserId)
                .Where(x => x.Gender == userParams.Gender);
            
            if (userParams.MinAge != 18 || userParams.MaxAge != 99){
                var minDoB = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDoB = DateTime.Today.AddYears(-userParams.MinAge - 1);

                users = users.Where(x => x.DateOfBirth >= minDoB && x.DateOfBirth <= maxDoB);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy)){
                switch (userParams.OrderBy){
                    case "created": 
                        users = users.OrderByDescending(x => x.Created);
                        break;
                    default:
                        users = users.OrderByDescending(x => x.LastActive);
                        break;
                }
            }
                
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}