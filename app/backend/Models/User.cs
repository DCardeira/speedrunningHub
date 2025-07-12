using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SpeedRunningHub.Models {
    public class User : IdentityUser {
        public ICollection<SpeedrunRecord> SpeedrunRecords { get; set; } = new List<SpeedrunRecord>();
        public ICollection<Guide> Guides { get; set; } = new List<Guide>();
    }
}