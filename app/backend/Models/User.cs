namespace SpeedRunningHub.Models{
    public class User{
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<SpeedrunRecord> SpeedrunRecords { get; set; }
        public ICollection<Guide> Guides { get; set; }
    }
}