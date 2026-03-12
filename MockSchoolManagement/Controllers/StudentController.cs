//using Microsoft.AspNetCore.Mvc;
//using MockSchoolManagement.Interface;
//using MockSchoolManagement.Models;

//namespace MockSchoolManagement.Controller
//{
//    public class StudentController : Controller
//    {
//        private IStudentRepository _studentRepository;
//        public StudentController(IStudentRepository studentRepository)
//        {
//            _studentRepository = studentRepository;
//        }

//        public IActionResult Details(int id)
//        {
//            Student model = _studentRepository.GetStudent(id);
//            return View(model);
//        }
//    }
//}
