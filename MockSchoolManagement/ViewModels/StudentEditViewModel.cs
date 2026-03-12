namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 编辑学生视图模型
    /// </summary>
    public class StudentEditViewModel: StudentCreateViewModel
    {
        public int Id { get; set; }

        /// <summary>
        /// 已经存在数据库中的头像路径
        /// </summary>
        public string? ExistingPhotoPath { get; set; }
    }
}
