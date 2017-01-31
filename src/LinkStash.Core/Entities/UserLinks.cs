using System;

namespace LinkStash.Core
{
    public class UserLinks
    {
        public int UserId { get; set; }

        public int LinkId { get; set; }

        public string Type { get; set; } // favorite, bookmark, followed

        public DateTime Created { get; set; }
    }
}
