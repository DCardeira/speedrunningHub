using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpeedRunningHub.Models {
    public class SpeedrunRecord {
        [Key]
        public int RunId { get; set; }

        public string RecordId { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(Game))]
        public int GameId { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public TimeSpan Time { get; set; }
        public bool IsApproved { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string VideoLink { get; set; } = null!;

        public Game Game { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}