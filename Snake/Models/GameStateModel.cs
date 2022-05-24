using System;

namespace Snake.Models
{
    public record class GameStateModel
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        public int Points { get; set; }
        public DateTime Date { get; set; }
    }
}
