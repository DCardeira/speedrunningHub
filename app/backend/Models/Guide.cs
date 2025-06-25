namespace SpeedRunningHub.Models{
    public class Guide{
        public int GuideId { get; set; }
        public int GameId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        public DateTime DateCreated { get; set; }

        public Game Game { get; set; }
        public User User { get; set; }
        public ICollection<GuideImage> GuideImages { get; set; }
    }
}