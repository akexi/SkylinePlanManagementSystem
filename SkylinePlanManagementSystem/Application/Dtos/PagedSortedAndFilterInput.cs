using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.Application.Dtos
{
    public class PagedSortedAndFilterInput
    {
        private const int DefaultCurrentPage = 1;
        private const int DefaultMaxResultCount = 10;

        public PagedSortedAndFilterInput()
        {
            CurrentPage = DefaultCurrentPage;
            MaxResultCount = DefaultMaxResultCount;
            Sorting = string.Empty;
            FilterText = string.Empty;
        }

        /// <summary>
        /// 每页分页条数，最大值为1000
        /// </summary>
        [Range(1,1000)]
        public int MaxResultCount { get; set; }

        /// <summary>
        /// 当前页，最大值为1000
        /// </summary>
        [Range(1,1000)]
        public int CurrentPage { get; set; }

        /// <summary>
        /// 排序字段ID
        /// </summary>
        public string Sorting { get; set; }

        /// <summary>
        /// 查询名称
        /// </summary>
        public string FilterText { get; set; }

    }
}
