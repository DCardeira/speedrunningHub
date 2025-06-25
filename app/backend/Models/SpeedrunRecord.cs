namespace SpeedRunningHub.Models{
    public class SpeedrunRecord{
        public int RunId { get; set; }
        public int GameId { get; set; }
        public string UserId { get; set; }
        public TimeSpan Time { get; set; }
        public bool IsApproved { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string VideoLink { get; set; }

        public Game Game { get; set; }
        public User User { get; set; }
    }
}