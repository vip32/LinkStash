namespace LinkStash.Core
{
    using System;

    public class UserLink : Entity
    {
        public int UserId { get; set; }

        public int LinkId { get; set; }

        public string Category { get; set; } // favorite, bookmark, followed

        public string[] Tags { get; set; }
    }
}
