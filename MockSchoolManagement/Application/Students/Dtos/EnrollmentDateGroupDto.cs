using System.ComponentModel.DataAnnotations;

namespace SkylinePlanManagementSystem.Application.Students.Dtos
{
    /// <summary>
    /// 入学时间分组Dto
    /// </summary>
    public class EnrollmentDateGroupDto
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EnrollmentDate { get; set; }

        public int StudentCount { get; set; }
    }
}
