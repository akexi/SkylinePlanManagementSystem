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
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 1,
                    Name = "李天喜",
                    Major = MajorEnum.ComputerScience,
                    Email = "xsbnltx@gmail.com",
                    PhotoPath = "default.jpg"
                }
            );
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 2,
                    Name = "张三",
                    Major = MajorEnum.Mathematics,
                    Email = "zhangsan@163.com",
                    PhotoPath = "default.jpg"
                }
            );
        }
    }
}
