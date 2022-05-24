using Snake.Data.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace Snake.Data.Models
{
    public record class User : BaseEntity<Guid>
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(64)]
        public byte[] Password { get; set; } = new byte[64];
    }
}
