namespace SpeedRunningHub.Models{
    public class GuideImage{
        public int GuideImageId { get; set; }
        public int GuideId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedByUserId { get; set; }

        public Guide Guide { get; set; }
        public User UploadedBy { get; set; }
    }
}