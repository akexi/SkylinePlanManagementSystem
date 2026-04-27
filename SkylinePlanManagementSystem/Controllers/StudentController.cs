//using Microsoft.AspNetCore.Mvc;
//using SkylinePlanManagementSystem.Interface;
//using SkylinePlanManagementSystem.Models;

//namespace SkylinePlanManagementSystem.Controller
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
