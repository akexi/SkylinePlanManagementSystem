using SkylinePlanManagementSystem.Models;

namespace SkylinePlanManagementSystem.DataRepositories
{
    public interface IStudentRepository
    {
        /// <summary>
        /// 通过id获取学生信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Student GetStudentById(int id);


        /// <summary>
        /// 获取所有学生信息
        /// </summary>
        /// <returns></returns>
        IEnumerable<Student> GetAllStudents();

        /// <summary>
        /// 添加学生信息
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        Student Insert(Student student);

        /// <summary>
        /// 修改学生信息
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        Student Update(Student student);

        /// <summary>
        /// 删除学生信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Student Delete(int id);


        //Student GetStudent(int id);
        //Student Add(Student student);
        //void Save(Student student);
    }
}
