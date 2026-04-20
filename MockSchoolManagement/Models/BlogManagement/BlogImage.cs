using Microsoft.EntityFrameworkCore.Proxies.Internal;

namespace MockSchoolManagement.Models.BlogManagement
{
    public class BlogImage
    {
        public int BlogImageId { get; set; }
        public byte[] Image { get; set; }
        public string Description { get; set; }
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
