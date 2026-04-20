using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;
using MockSchoolManagement.Models.EnumTypes;
using System.Reflection.PortableExecutable;

namespace MockSchoolManagement.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        // 将需要初始化到数据库的模型数据写在此方法内
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // 指定实体在数据库中生成的名称
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<StudentCourse>().ToTable("StudentCourse");
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<CourseAssignment>().HasKey(c => new { c.CourseId, c.TeacherId });

            modelBuilder.Entity<Blog>().ToTable("Blogs").HasKey(a => a.Id);
            modelBuilder.Entity<Blog>().Property(a => a.Title).HasMaxLength(50).HasColumnName("BlogTitle");
        }
    }
}
