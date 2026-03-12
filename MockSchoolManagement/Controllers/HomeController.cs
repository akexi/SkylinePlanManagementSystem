using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;

namespace MockSchoolManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger logger;
        // IDataProtector提供了Protect()和Unprotect()方法来加密和解密数据
        private readonly IDataProtector protector;

        // 使用构造函数注入的方式注入IStudentRepository、IWebHostEnvironment和ILogger服务
        public HomeController(IStudentRepository studentRepository, 
            IWebHostEnvironment webHostEnvironment, 
            ILogger<HomeController> logger, 
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _studentRepository = studentRepository;
            _webHostEnvironment = webHostEnvironment;
            this.logger = logger;
            this.protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.StudentIdRouteValue);
        }

        //[Route("")]
        //[Route("Home")]
        //[Route("Home/Index")]
        public ViewResult Index()
        {
            // 查询所有的学生信息
            List<Student> model = _studentRepository.GetAllStudents().Select(s =>
            {
                // 加密ID值并存储在EncryptedId属性中
                s.EncryptedId = protector.Protect(s.Id.ToString());
                return s;
            }).ToList();
            // 将学生列表传递到视图
            return View(model);
        }

        //[Route("Home/Details/{id}")]
        // Details视图接收加密后的学生ID
        public ViewResult Details(string id)
        {
            // 使用Unprotect()方法解密学生ID值
            string decryptedId = protector.Unprotect(id);
            int decryptedStudentId = Convert.ToInt32(decryptedId);

            var student = _studentRepository.GetStudentById(decryptedStudentId);
            // 判断学生是否存在，如果不存在则返回404错误页面
            if (student == null)
            {
                ViewBag.ErrorMessage = $"学生Id={id}的信息不存在，请重试";
                return View("NotFound");
            }
            // 实例化HomeDatilsViewModel并存储Student详细信息和PageTitle
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Student = student,
                PageTitle = "学生详情"
            };

            // 将ViewModel对象传递给View()方法
            return View(homeDetailsViewModel);
        }

        //[Route("Home/Create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    // 循环每个选定上传的文件
                    foreach (IFormFile photo in model.Photos)
                    {
                        // 必须将图片文件上传到wwwroot的images文件夹中
                        // 而要获取wwwroot文件夹的路径，需注入WebHostEnvironment服务获取wwwroot文件夹路径
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                        // 为了确保文件名唯一，在文件名后附加一个新的GUID值
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        // 使用IFormFile接口提供的CopyTo()方法将文件复制到wwwroot/images文件夹
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }

                }
                Student newStudent = new Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    Major = model.Major,
                    // 文件名保存在Student对象的PhotoPath属性中
                    // 它将被存储在数据库Student表的PhotoPath字段中
                    PhotoPath = uniqueFileName
                };
                _studentRepository.Insert(newStudent);
                return RedirectToAction("Details", new { id = newStudent.Id });
            }
            return View();
        }

        [HttpGet]
        public ViewResult Edit(string id)
        {
            // 使用Unprotect()方法解密学生ID值
            string decryptedId = protector.Unprotect(id);
            int decryptedStudentId = Convert.ToInt32(decryptedId);

            Student student = _studentRepository.GetStudentById(decryptedStudentId);

            // 判断学生是否存在，如果不存在则返回404错误页面
            if (student == null)
            {
                Response.StatusCode = 404;
                return View("StudentNotFound", id);
            }

            StudentEditViewModel studentEditViewModel = new StudentEditViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Major = student.Major,
                ExistingPhotoPath = student.PhotoPath
            };
            return View(studentEditViewModel);
        }

        // StudentEditViewModel会接收来自POST请求的Edit表单数据
        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            // 检查提供的数据是否有效，如果无效需要重新编辑学生信息
            if (ModelState.IsValid)
            {
                // 从数据库中获取要编辑的学生信息
                Student student = _studentRepository.GetStudentById(model.Id);
                // 用模型数据更新student对象
                student.Name = model.Name;
                student.Email = model.Email;
                student.Major = model.Major;

                // 如果用户上传了新的照片,模型的Photos属性将接收到上传的文件
                // 如果没有上传新照片,保留现有的图片文件信息
                // 因为兼容了多图片上传,所以判断总数是否大于0
                if (model.Photos.Count > 0)
                {
                    // 如果上传了新照片,则显示新照片并删除旧照片
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    // 上传新照片并更新student对象的PhotoPath属性
                    student.PhotoPath = ProcessUploadedFile(model);
                }
                // 调用存储库的Update()方法更新学生信息
                Student updatedStudent = _studentRepository.Update(student);
                return RedirectToAction("index");
            }

            return View(model);
        }

        /// <summary>
        /// 将图片保存到wwwroot/images/avatars文件夹中，并返回唯一的文件名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ProcessUploadedFile(StudentCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photos.Count > 0)
            {
                foreach (var photo in model.Photos)
                {
                    // 必需将图片文件上传到wwwroot的images/avatars文件夹中
                    // 而要获取wwwroot文件夹的路径，需注入WebHostEnvironment服务获取wwwroot文件夹路径
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                    // 为了确保文件名唯一，在文件名后附加一个新的GUID值
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    // 因为使用了非托管资源,所以需要手动释放
                    using (var fileStream = new FileStream(filePath,FileMode.Create))
                    {
                        // 使用IFormFile接口提供的CopyTo()方法将文件复制到wwwroot/images/avatars文件夹
                        photo.CopyTo(fileStream);
                    }
                }
            }

            return uniqueFileName;
        }

    }
}
