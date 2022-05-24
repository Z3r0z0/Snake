using Microsoft.EntityFrameworkCore;
using Snake.Data.DataAccessLayers.Base;
using Snake.Data.Models;
using Snake.Extensions;
using Snake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable disable

namespace Snake.Data.DataAccessLayers
{
    public class UserDataAccessLayer : ContextDataAccessLayer
    {
        public UserDataAccessLayer(SnakeContext context) : base(context)
        {
        }

        public async Task<List<UserModel>> GetAll()
        {
            return await _context.Set<User>()
                .Select(x => new UserModel
                {
                    Id = x.Id,
                    Username = x.Username,
                }).ToListAsync();
        }

        public async Task<UserModel> GetById(Guid id)
        {
            return await _context.Set<User>()
                .Select(x => new UserModel
                {
                    Id = x.Id,
                    Username = x.Username,
                }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Add(UserModel model)
        {
            _context.Add(new User
            {
                Username = model.Username,
                Password = model.Password.HashString()
            });

            await _context.SaveChangesAsync();
        }

        public async Task Update(UserModel model)
        {
            var data = await _context.Set<User>().FirstOrDefaultAsync(x => x.Id == model.Id);

            if (data is null)
                return;

            data.Username = model.Username;
            data.Password = model.Password.HashString();
            
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var data = await _context.Set<User>().FirstOrDefaultAsync(x => x.Id == id);

            if (data is null)
                return;

            _context.Remove(data);

            await _context.SaveChangesAsync();
        }
    }
}
