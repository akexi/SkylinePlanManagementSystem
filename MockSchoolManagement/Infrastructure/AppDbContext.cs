using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Models;
using MockSchoolManagement.Models.EnumTypes;

namespace MockSchoolManagement.Infrastructure
{
    // 注意：将ApplicationUser作为泛型参数传递给IdentityDbContext
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        // 定义Students数据集
        public DbSet<Student> Students { get; set; }

        // 方法内调用Seed创建初始数据方法
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Seed();    // 如果需要添加其他实体的数据初始化，可在此调用Seed方法，具体实现见ModelBuilderExtensions类

            // 获取当前系统中所有领域模型上的外键列表
            var foreignKeys = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());

            foreach(var foreignKey in foreignKeys)
            {
                // 将它们的删除行为配置为Restrict，即无操作
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

    }
}
