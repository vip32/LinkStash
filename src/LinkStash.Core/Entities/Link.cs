using System;

namespace LinkStash.Core
{
    public class Link
    {
        public int Id { get; set; }

        public string Hash { get; set; }

        public string Group { get; set; }

        public string Source { get; set; }

        public string Href { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public DateTime Created { get; set; }
    }
}
