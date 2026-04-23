using SkylinePlanManagementSystem.Models;
using System.Diagnostics.Contracts;

namespace SkylinePlanManagementSystem.Application.Dtos
{
    public class PagedResultDto<TEntity> :PagedSortedAndFilterInput
    {
        /// <summary>
        /// 数据总合计
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(TotalCount, MaxResultCount));

        public List<TEntity> Data { get; set; }

        /// <summary>
        /// 是否显示上一页按钮
        /// </summary>
        public bool ShowPrevious => CurrentPage > 1;

        /// <summary>
        /// 是否显示下一页按钮
        /// </summary>
        public bool ShowNext => CurrentPage < TotalPages;

        /// <summary>
        /// 是否显示第一页按钮
        /// </summary>
        public bool ShowFirst => CurrentPage != 1;

        /// <summary>
        /// 是否显示最后一页按钮
        /// </summary>
        public bool ShowLast => CurrentPage != TotalPages;

    }
}
