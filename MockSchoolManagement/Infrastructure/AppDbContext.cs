using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;
using MockSchoolManagement.Models.BlogManagement;
using MockSchoolManagement.Models.EnumTypes;
using System.Reflection.PortableExecutable;

namespace MockSchoolManagement.Infrastructure
{
    // 注意：将ApplicationUser作为泛型参数传递给IdentityDbContext
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        // 定义数据集属性，表示数据库中的表
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<OfficeLocation> OfficeLocations { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        // 方法内调用Seed创建初始数据方法
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();    // 如果需要添加其他实体的数据初始化，可在此调用Seed方法，具体实现见ModelBuilderExtensions类

            // 获取当前系统中所有领域模型上的外键列表
            var foreignKeys = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach(var foreignKey in foreignKeys)
            {
                // 将它们的删除行为配置为Restrict，即无操作
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Blog与Post之间为一对多关联关系
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Blog)
                .WithMany(b => b.Posts)
                .HasForeignKey(p => p.BId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }

        // 由于我们在Program.cs中已经通过AddDbContextPool方法配置了DbContext，因此这里不需要再重写OnConfiguring方法来设置连接字符串和数据库提供程序。
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder
        //        .UseLazyLoadingProxies()
        //        .UseMySql(_configuration.GetConnectionString("MockStudentDBConnection"));
        //}

    }
}
