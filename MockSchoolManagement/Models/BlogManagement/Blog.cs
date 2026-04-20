using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MockSchoolManagement.Models.BlogManagement
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string BloggerName { get; set; }
        public virtual BlogImage BlogImage { get; set; }
        public virtual List<Post> Posts { get; set; }
    }
}
