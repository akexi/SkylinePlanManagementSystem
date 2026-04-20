using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockSchoolManagement.Models.BlogManagement;

namespace MockSchoolManagement.Infrastructure.EntityMapper
{
    public class BlogMapper:IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("Blog");

            // 主键
            builder.HasKey(t => t.Id);

            // 设置Title属性最大长度为70，列名在数据库中显示为BlogTitle
            builder.Property(a => a.Title)
                .HasMaxLength(70)
                .HasColumnName("BlogTitle");

        }
    }
}
