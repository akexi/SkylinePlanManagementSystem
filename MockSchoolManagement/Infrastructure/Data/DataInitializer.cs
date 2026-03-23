using Microsoft.AspNetCore.Identity;
using MockSchoolManagement.Models;
using MockSchoolManagement.Models.EnumTypes;

namespace MockSchoolManagement.Infrastructure.Data
{
    public static class DataInitializer
    {
        public static IApplicationBuilder UseDataInitializer(this IApplicationBuilder builder)
        {
            using(var scope = builder.ApplicationServices.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                #region 学生种子信息

                if (dbcontext.Students.Any())
                {
                    return builder; // 数据已经存在，跳过数据初始化
                }

                var students = new[]
                {
                    new Student { Name = "张三", Major = MajorEnum.ComputerScience, Email = "zhangsan@52abp.com", EnrollmentDate = DateTime.Parse("2020-09-01"), PhotoPath = "noimage.png" },
                    new Student { Name = "李四", Major = MajorEnum.ComputerScience, Email = "lisi@52abp.com", EnrollmentDate = DateTime.Parse("2020-09-02"), PhotoPath = "noimage.png" },
                    new Student { Name = "王五", Major = MajorEnum.ComputerScience, Email = "wangwu@52abp.com", EnrollmentDate = DateTime.Parse("2012-08-31"), PhotoPath = "noimage.png" }
                };

                foreach(Student item in students)
                {
                    dbcontext.Students.Add(item);
                }
                dbcontext.SaveChanges();

                #endregion 学生种子信息


                #region 课程种子数据

                if(dbcontext.Courses.Any())
                {
                    return builder; // 数据已经存在，跳过数据初始化
                }
                var courses = new[]
                {
                    new Course { CourseId = 1050, Title = "数据结构", Credits = 3 },
                    new Course { CourseId = 4022, Title = "操作系统", Credits = 4 },
                    new Course { CourseId = 4041, Title = "数据库系统", Credits = 3 },
                    new Course { CourseId = 1045, Title = "计算机网络", Credits = 4 },
                    new Course { CourseId = 3141, Title = "人工智能", Credits = 3 },
                    new Course { CourseId = 2021, Title = "编译原理", Credits = 4 },
                    new Course { CourseId = 2042, Title = "软件工程", Credits = 3 },
                };

                foreach(var c in courses)
                {
                    dbcontext.Courses.Add(c);
                }
                dbcontext.SaveChanges();

                #endregion 课程种子数据

                #region 学生课程关联种子数据
                // 这里学生的ID为4、5、6是因为之前的种子数据中已经占了1、2、3的ID了
                var studentCourses = new[]
                {
                    new StudentCourse { CourseId = 1050, StudentId = 1 },
                    new StudentCourse { CourseId = 4022, StudentId = 2 },
                    new StudentCourse { CourseId = 4041, StudentId = 3 },
                    new StudentCourse { CourseId = 1045, StudentId = 1 },
                    new StudentCourse { CourseId = 3141, StudentId = 2 },
                    new StudentCourse { CourseId = 2021, StudentId = 3 },
                    new StudentCourse { CourseId = 1050, StudentId = 1 },
                };

                foreach(var sc in studentCourses)
                {
                    dbcontext.StudentCourses.Add(sc);
                }
                dbcontext.SaveChanges();

                #endregion 学生课程关联种子数据

                #region 用户和角色种子数据
                
                if(userManager.Users.Any())
                {
                    return builder; // 用户或角色已经存在，跳过数据初始化
                }

                var user = new ApplicationUser { UserName = "admin", Email = "admin@ddxc.org", EmailConfirmed = true, City = "昆明" };
                userManager.CreateAsync(user, "Admin@123").Wait();  // 等待异步操作完成
                dbcontext.SaveChanges();
                var adminRole = "Admin";

                var role = new IdentityRole { Name = adminRole };

                dbcontext.Roles.Add(role);
                dbcontext.SaveChanges();

                dbcontext.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = role.Id });
                dbcontext.SaveChanges();

                #endregion 用户和角色种子数据

            }

            return builder;
        }
    }
}
