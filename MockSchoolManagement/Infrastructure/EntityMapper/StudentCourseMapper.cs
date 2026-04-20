using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MockSchoolManagement.Models;

namespace MockSchoolManagement.Infrastructure.EntityMapper
{
    public class StudentCourseMapper:IEntityTypeConfiguration<StudentCourse>
    {
        public void Configure(EntityTypeBuilder<StudentCourse> builder)
        {
            // 修改表名为Enrollment，设置StudentCourseId为主键
            builder.ToTable("Enrollment")
                .HasKey(a => a.StudentCourseId);

            // StudentCourse关联实体Student，设置外键ID为StudentId
            builder.HasOne(a => a.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(a => a.StudentId);

            builder.HasOne(a => a.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(a => a.CourseId);
        }
    }
}
