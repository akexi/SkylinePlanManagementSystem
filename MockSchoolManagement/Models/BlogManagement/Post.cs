using System.ComponentModel.DataAnnotations.Schema;

namespace MockSchoolManagement.Models.BlogManagement
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int BId { get; set; }
        public virtual Blog Blog { get; set; }
    }
}
