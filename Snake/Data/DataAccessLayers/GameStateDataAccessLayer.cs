using Microsoft.EntityFrameworkCore;
using Snake.Data.DataAccessLayers.Base;
using Snake.Data.Models;
using Snake.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable disable

namespace Snake.Data.DataAccessLayers
{
    public class GameStateDataAccessLayer : ContextDataAccessLayer
    {
        public GameStateDataAccessLayer(SnakeContext context) : base(context)
        {
        }

        public async Task<List<GameStateModel>> GetAll()
        {
            return await _context.Set<GameState>()
                .Select(x => new GameStateModel
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Points = x.Points,
                    Date = x.Date,
                }).ToListAsync();
        }

        public async Task<GameStateModel> GetById(int id)
        {
            return await _context.Set<GameState>()
                .Select(x => new GameStateModel
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Points = x.Points,
                    Date = x.Date,
                }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<GameStateModel>> GetTopTen()
        {
            return await _context.Set<GameState>()
                .Select(x => new GameStateModel
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Username = x.User.Username,
                    Date = x.Date,
                    Points = x.Points
                })
                .OrderByDescending(x => x.Points)
                .Take(10)
                .ToListAsync();
        }

        public async Task Add(GameStateModel model)
        {
            _context.Add(new GameState
            {
                UserId = model.UserId,
                Points = model.Points,
                Date = model.Date,
            });

            await _context.SaveChangesAsync();
        }

        public async Task Update(GameStateModel model)
        {
            var data = await _context.Set<GameState>().FirstOrDefaultAsync(x => x.Id == model.Id);

            if (data is null)
                return;

            data.UserId = model.UserId;
            data.Points = model.Points;
            data.Date = model.Date;
        }

        public async Task Delete(int id)
        {
            var data = await _context.Set<GameState>().FirstOrDefaultAsync(x => x.Id == id);

            if (data is null)
                return;

            _context.Remove(data);

            await _context.SaveChangesAsync();
        }
    }
}
