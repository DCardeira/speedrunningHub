namespace SpeedRunningHub.Models{
    public class Role{
        public int RoleId { get; set; }
        public string Name { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}