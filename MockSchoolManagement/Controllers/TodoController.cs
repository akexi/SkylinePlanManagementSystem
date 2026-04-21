using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;

namespace MockSchoolManagement.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TodoController : ControllerBase
    {
        // 注入仓储服务，TodoItem的主键Id为long类型，仓储服务参数也需要使用long类型
        private readonly IRepository<TodoItem, long> _todoItemRepository;

        public TodoController(IRepository<TodoItem, long> todoItemRepository)
        {
            _todoItemRepository = todoItemRepository;
        }

        /// <summary>
        /// 获取所有的待办事项列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> GetAll()
        {
            var models = await _todoItemRepository.GetAllListAsync();

            return models;
        }

        #region 根据Id获取待办事项

        /// <summary>
        /// 通过id获取待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetById(long id)
        {
            var todoItem = await _todoItemRepository.FirstOrDefaultAsync(a => a.Id == id);

            if(todoItem == null)
            {
                // 返回 404
                return NotFound();
            }
            
            return todoItem;
        }

        #endregion

        #region 更新待办事项

        /// <summary>
        /// 更新待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="todoItem"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id,TodoItem todoItem)
        {
            if(id != todoItem.Id)
            {
                return BadRequest();
            }

            await _todoItemRepository.UpdateAsync(todoItem);

            // 返回204 No Content，表示更新成功但没有返回内容
            return NoContent();
        }

        #endregion

        #region 添加待办事项

        /// <summary>
        /// 添加待办事项
        /// </summary>
        /// <param name="todoItem"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItem>> Create(TodoItem todoItem)
        {
            await _todoItemRepository.InsertAsync(todoItem);

            // 创建一个reatedAtActionResult对象，它生成一个状态码为Status201Created的HTTP响应
            return CreatedAtAction(nameof(GetAll), new { id = todoItem.Id }, todoItem);
        }

        #endregion

        #region 删除指定Id的待办事项

        /// <summary>
        /// 删除指定Id的待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> Delete(long id)
        {
            var todoItem = await _todoItemRepository.FirstOrDefaultAsync(a => a.Id == id);

            if(todoItem == null)
            {
                return NotFound();
            }

            await _todoItemRepository.DeleteAsync(todoItem);

            return todoItem;
        }

        #endregion




    }
}
