namespace SpeedRunningHub.Models {
    public class Game {
        public int GameId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; } 
        public ICollection<SpeedrunRecord> SpeedrunRecords { get; set; }
        public ICollection<Guide> Guides { get; set; }
        public ICollection<GameImage> GameImages { get; set; }
    }
}