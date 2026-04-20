using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MockSchoolManagement.Models
{
    [Table(name: "Blogs")]
    public class Blog
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "BlogTitle")]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }
        public string BloggerName { get; set; }
    }
}
