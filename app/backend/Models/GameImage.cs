namespace SpeedRunningHub.Models{
    public class GameImage{
        public int GameImageId { get; set; }
        public int GameId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedByUserId { get; set; }

        public Game Game { get; set; }
        public User UploadedBy { get; set; }
    }
}