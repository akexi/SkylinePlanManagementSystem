using Microsoft.EntityFrameworkCore;
using SkylinePlanManagementSystem.Infrastructure;
using SkylinePlanManagementSystem.Models.EnumTypes;

namespace SkylinePlanManagementSystem.Application.Projects
{
    public class ProjectProgressBackgroundService: BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ProjectProgressBackgroundService> _logger;

        public ProjectProgressBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<ProjectProgressBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            await UpdateProgressAsync(stoppingToken);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await UpdateProgressAsync(stoppingToken);
            }
        }

        private async Task UpdateProgressAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var nodeProgressData = await dbContext.ProjectNodes
                    .Select(n => new
                    {
                        n.ProjectNodeId,
                        Completed = n.SubNodes.Count(sn => sn.ProgressStatus == SubNodeProgressStatus.已完成),
                        Total = n.SubNodes.Count()
                    })
                    .ToListAsync(cancellationToken);

                var nodeProgressMap = nodeProgressData.ToDictionary(
                    x => x.ProjectNodeId,
                    x => x.Total == 0 ? 0d : Math.Round((double)x.Completed * 100d / x.Total, 2));

                var nodes = await dbContext.ProjectNodes.ToListAsync(cancellationToken);
                foreach (var node in nodes)
                {
                    node.CompletionProgress = nodeProgressMap.GetValueOrDefault(node.ProjectNodeId, 0d);
                }

                var projectProgressData = await dbContext.Projects
                    .Select(p => new
                    {
                        p.ProjectId,
                        Completed = p.Nodes.SelectMany(n => n.SubNodes).Count(sn => sn.ProgressStatus == SubNodeProgressStatus.已完成),
                        Total = p.Nodes.SelectMany(n => n.SubNodes).Count()
                    })
                    .ToListAsync(cancellationToken);

                var projectProgressMap = projectProgressData.ToDictionary(
                    x => x.ProjectId,
                    x => x.Total == 0 ? 0d : Math.Round((double)x.Completed * 100d / x.Total, 2));

                var projects = await dbContext.Projects.ToListAsync(cancellationToken);
                foreach (var project in projects)
                {
                    project.CompletionProgress = projectProgressMap.GetValueOrDefault(project.ProjectId, 0d);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "项目进度批处理更新失败");
            }
        }
    }
}
