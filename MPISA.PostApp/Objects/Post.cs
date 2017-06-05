using System.Collections.Generic;

namespace MPISA.PostApp.Objects
{
    public class Post
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public List<Link> Links { get; set; }
        public List<string> Embeds { get; set; }
    }
}
