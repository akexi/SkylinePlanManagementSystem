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
                    new Student { Name = "李四", Major = MajorEnum.Mathematics, Email = "lisi@52abp.com", EnrollmentDate = DateTime.Parse("2020-09-02"), PhotoPath = "noimage.png" },
                    new Student { Name = "王五", Major = MajorEnum.ElectronicCommerce, Email = "wangwu@52abp.com", EnrollmentDate = DateTime.Parse("2012-08-31"), PhotoPath = "noimage.png" }
                };

                foreach(Student item in students)
                {
                    dbcontext.Students.Add(item);
                }
                dbcontext.SaveChanges();

                #endregion 学生种子信息

                #region 学院种子数据

                var teachers = new[]
                {
                    new Teacher { Name = "赵老师", HireDate = DateTime.Parse("2010-03-15") },
                    new Teacher { Name = "钱老师", HireDate = DateTime.Parse("2012-07-01") },
                    new Teacher { Name = "孙老师", HireDate = DateTime.Parse("2015-09-01") },
                    new Teacher { Name = "李老师", HireDate = DateTime.Parse("2018-01-10") },
                    new Teacher { Name = "周老师", HireDate = DateTime.Parse("2020-11-20") },
                    new Teacher { Name = "吴老师", HireDate = DateTime.Parse("2019-05-05") },
                    new Teacher { Name = "郑老师", HireDate = DateTime.Parse("2011-04-25") },
                    new Teacher { Name = "王老师", HireDate = DateTime.Parse("2013-06-30") },
                    new Teacher { Name = "胡老师", HireDate = DateTime.Parse("2016-10-15") },
                    new Teacher { Name = "刘老师", HireDate = DateTime.Parse("2014-02-20") },
                };

                foreach(var t in teachers)
                {
                    dbcontext.Teachers.Add(t);
                }
                dbcontext.SaveChanges();

                #endregion 学院种子数据

                var departments = new[]
                {
                    new Department { Name = "a", Budget = 350000, StartDate = DateTime.Parse("2017-09-01"), TeacherId = teachers.Single(i => i.Name == "刘老师").Id },
                    new Department { Name = "b", Budget = 100000, StartDate = DateTime.Parse("2017-09-01"), TeacherId = teachers.Single(i => i.Name == "赵老师").Id },
                    new Department { Name = "c", Budget = 350000, StartDate = DateTime.Parse("2017-09-01"), TeacherId = teachers.Single(i => i.Name == "胡老师").Id },
                    new Department { Name = "d", Budget = 100000, StartDate = DateTime.Parse("2017-09-01"), TeacherId = teachers.Single(i => i.Name == "王老师").Id },

                };
                foreach(var d in departments)
                {
                    dbcontext.Departments.Add(d);
                }
                dbcontext.SaveChanges();


                #region 课程种子数据

                if (dbcontext.Courses.Any())
                {
                    return builder; // 数据已经存在，跳过数据初始化
                }
                var courses = new[]
                {
                    new Course { CourseId = 1050, Title = "数据结构", Credits = 3, DepartmentId = departments.Single(s => s.Name == "a").DepartmentId },
                    new Course { CourseId = 4022, Title = "算法分析", Credits = 3, DepartmentId = departments.Single(s => s.Name == "a").DepartmentId },
                    new Course { CourseId = 4041, Title = "操作系统", Credits = 3, DepartmentId = departments.Single(s => s.Name == "b").DepartmentId },
                    new Course { CourseId = 1045, Title = "微积分", Credits = 4, DepartmentId = departments.Single(s => s.Name == "b").DepartmentId },
                    new Course { CourseId = 3141, Title = "线性代数", Credits = 4, DepartmentId = departments.Single(s => s.Name == "c").DepartmentId },
                    new Course { CourseId = 2021, Title = "市场营销", Credits = 3, DepartmentId = departments.Single(s => s.Name == "c").DepartmentId },
                    new Course { CourseId = 2042, Title = "财务会计", Credits = 3, DepartmentId = departments.Single(s => s.Name == "d").DepartmentId },
                    new Course { CourseId = 2043, Title = "管理学", Credits = 3, DepartmentId = departments.Single(s => s.Name == "d").DepartmentId },
                };

                foreach (var c in courses)
                {
                    dbcontext.Courses.Add(c);
                }
                dbcontext.SaveChanges();

                #endregion 课程种子数据

                #region 办公室分配种子数据

                var OfficeLocations = new[]
                {
                    new OfficeLocation{ TeacherId = teachers.Single(i => i.Name == "刘老师").Id, Location = "X楼"},
                    new OfficeLocation{ TeacherId = teachers.Single(i => i.Name == "胡老师").Id, Location = "Y楼"},
                    new OfficeLocation{ TeacherId = teachers.Single(i => i.Name == "王老师").Id, Location = "Z楼"},
                };
                foreach(var o in OfficeLocations)
                {
                    dbcontext.OfficeLocations.Add(o);
                }
                dbcontext.SaveChanges();

                #endregion 办公室分配种子数据

                #region 为教师分配课程的种子数据

                var courseTeachers = new[]
                {
                    new CourseAssignment{
                        CourseId = courses.Single(c => c.Title == "数据结构").CourseId,
                        TeacherId = teachers.Single(i => i.Name == "赵老师").Id
                    },
                    new CourseAssignment{
                        CourseId = courses.Single(c => c.Title == "算法分析").CourseId,
                        TeacherId = teachers.Single(i => i.Name == "钱老师").Id
                    },
                    new CourseAssignment{
                        CourseId = courses.Single(c => c.Title == "操作系统").CourseId,
                        TeacherId = teachers.Single(i => i.Name == "孙老师").Id
                    },
                    new CourseAssignment{
                        CourseId = courses.Single(c => c.Title == "微积分").CourseId,
                        TeacherId = teachers.Single(i => i.Name == "李老师").Id
                    },
                    new CourseAssignment{
                        CourseId = courses.Single(c => c.Title == "线性代数").CourseId,
                        TeacherId = teachers.Single(i => i.Name == "周老师").Id
                    },
                    new CourseAssignment{
                        CourseId = courses.Single(c => c.Title == "市场营销").CourseId,
                        TeacherId = teachers.Single(i => i.Name == "郑老师").Id
                    },
                    new CourseAssignment{
                        CourseId = courses.Single(c => c.Title == "财务会计").CourseId,
                        TeacherId = teachers.Single(i => i.Name == "王老师").Id
                    },
                    new CourseAssignment{
                        CourseId = courses.Single(c => c.Title == "管理学").CourseId,
                        TeacherId = teachers.Single(i => i.Name == "胡老师").Id
                    },
                    
                };

                foreach(var ct in courseTeachers)
                {
                    dbcontext.CourseAssignments.Add(ct);
                }
                dbcontext.SaveChanges();

                #endregion 为教师分配课程的种子数据


                #region 学生课程关联种子数据
                // 这里学生的ID为4、5、6是因为之前的种子数据中已经占了1、2、3的ID了
                var studentCourses = new[]
                {
                    new StudentCourse { 
                        StudentId = students.Single(s => s.Name == "张三").Id,
                        CourseId = courses.Single(c => c.Title == "数据结构").CourseId,
                        Grade = Grade.A
                    },
                    new StudentCourse {
                        StudentId = students.Single(s => s.Name == "李四").Id,
                        CourseId = courses.Single(c => c.Title == "算法分析").CourseId,
                        Grade = Grade.B
                    },
                };

                foreach(var sc in studentCourses)
                {
                    dbcontext.StudentCourses.Add(sc);
                }
                dbcontext.SaveChanges();

                #endregion 学生课程关联种子数据

                #region 用户和角色种子数据
                
                if(dbcontext.Users.Any())
                {
                    return builder; // 用户或角色已经存在，跳过数据初始化
                }

                var user = new ApplicationUser { 
                    UserName = "admin@ddxc.org", 
                    Email = "admin@ddxc.org", 
                    EmailConfirmed = true, City = "昆明" 
                };

                userManager.CreateAsync(user, "Admin@123").Wait();  // 等待异步操作完成
                dbcontext.SaveChanges();

                var adminRole = "Admin";

                var role = new IdentityRole { Name = adminRole };

                dbcontext.Roles.Add(role);
                dbcontext.SaveChanges();

                dbcontext.UserRoles.Add(new IdentityUserRole<string> 
                { 
                    UserId = user.Id, 
                    RoleId = role.Id 
                });

                dbcontext.SaveChanges();

                #endregion 用户和角色种子数据

            }

            return builder;
        }
    }
}
