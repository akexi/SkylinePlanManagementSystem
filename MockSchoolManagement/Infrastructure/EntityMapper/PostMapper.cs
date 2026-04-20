using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockSchoolManagement.Models.BlogManagement;

namespace MockSchoolManagement.Infrastructure.EntityMapper
{
    public class PostMapper:IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            // Blog与Post之间的一对多关系
            builder.HasOne(p => p.Blog)
                   .WithMany(b => b.Posts)
                   .HasForeignKey(p => p.BId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Post");

            // 设置Title属性最大长度为50，列名在数据库中显示为Title
            builder.Property(a => a.Title)
                     .HasMaxLength(50)
                     .HasColumnName("Title");

            // 设置属性PostId，列名在数据库中显示为Id
            builder.Property(t => t.PostId)
                     .HasColumnName("Id");
        }
    }
}
