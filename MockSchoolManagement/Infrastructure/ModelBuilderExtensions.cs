using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;
using MockSchoolManagement.Models.EnumTypes;

namespace MockSchoolManagement.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        // 将需要初始化到数据库的模型数据写在此方法内
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // 指定实体在数据库中生成的名称
            modelBuilder.Entity<Course>().ToTable("School_Course");
            modelBuilder.Entity<StudentCourse>().ToTable("StudentCourse");
            modelBuilder.Entity<Student>().ToTable("Student");
        }
    }
}
