using Microsoft.EntityFrameworkCore;
using Snake.Data;
using Snake.Data.Models;
using Snake.Extensions;
using Snake.Models;
using System.Linq;
using System.Threading.Tasks;

#nullable disable

namespace Snake.Services
{
    public class LogOnService
    {
        private static LogOnService _instance;
        private static object _lock = new object();

        private readonly SnakeContext _context;

        private LogOnService(SnakeContext context)
        {
            _context = context;
        }

        public static LogOnService GetInstance(SnakeContext context)
        {
            if (_instance is null)
            {
                lock (_lock)
                {
                    if (_instance is null)
                    {
                        _instance = new LogOnService(context);
                    }
                }
            }

            return _instance;
        }

        public UserModel LogOnModel { get; set; }

        public bool IsLoggedIn { get => LogOnModel is not null; }

        public async Task Login(string username, string password)
        {
            LogOnModel = await _context.Set<User>()
                .Where(x => x.Username == username && x.Password == password.HashString())
                .Select(x => new UserModel
                {
                    Id = x.Id,
                    Username = x.Username
                }).FirstOrDefaultAsync();
        }

        public void Logout() => LogOnModel = null;
    }
}
