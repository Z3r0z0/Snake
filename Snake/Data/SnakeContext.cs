using Microsoft.EntityFrameworkCore;
using Snake.Data.Models;
using Snake.Data.Models.Base;
using Snake.Extensions;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Snake.Data
{
    public class SnakeContext : DbContext
    {
        private static bool _seeded = false;

        public SnakeContext(DbContextOptions options) : base(options)
        {
            if(!_seeded)
                Seed();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Loading the types which implement IEntity and add them to the context
            foreach (var type in typeof(IEntity).Assembly.GetExportedTypes()
                                            .Where(p => typeof(IEntity).IsAssignableFrom(p)))
                if (!type.IsAbstract && !type.IsInterface && type.IsClass)
                    modelBuilder.Entity(type);

            var relations = modelBuilder.Model.GetEntityTypes().SelectMany(c => c.GetForeignKeys());
            foreach (var rel in relations)
            {
                rel.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private void Seed()
        {
            if (Database.IsSqlServer())
                return;

            _seeded = true;

            using var sha = SHA256.Create();

            Add(new User
            {
                Username = "test",
                Password = ("test").HashString()
            });

            SaveChanges();
        }
    }
}
