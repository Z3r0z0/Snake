using Snake.Data.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace Snake.Data.Models
{
    public record class GameState : IdEntity
    {
        [Required]
        public User? User { get; set; }
        public Guid UserId { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
