namespace LinkStash.Core
{
    using System;

    public class Link : Entity
    {
        public string Group { get; set; }

        public string Source { get; set; }

        public string Href { get; set; }

        public string Title { get; set; }

        public string[] Tags { get; set; }

        public string Author { get; set; }
    }
}
