using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.Models.EnumTypes
{
    public enum MajorEnum
    {
        [Display(Name = "未分配")]
        None,
        [Display(Name = "计算机科学与技术")]
        ComputerScience,
        [Display(Name = "电子商务")]
        ElectronicCommerce,
        [Display(Name = "数学")]
        Mathematics
    }
}
