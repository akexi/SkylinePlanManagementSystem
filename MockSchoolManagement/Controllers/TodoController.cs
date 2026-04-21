using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;

namespace MockSchoolManagement.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        // 注入仓储服务，TodoItem的主键Id为long类型，仓储服务参数也需要使用long类型
        private readonly IRepository<TodoItem, long> _todoItemRepository;

        public TodoController(IRepository<TodoItem, long> todoItemRepository)
        {
            _todoItemRepository = todoItemRepository;
        }

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> GetTodo()
        {
            var models = await _todoItemRepository.GetAllListAsync();

            return (models);
        }

        #region 根据Id获取待办事项

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
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

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id,TodoItem todoItem)
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

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            await _todoItemRepository.InsertAsync(todoItem);

            // 创建一个reatedAtActionResult对象，它生成一个状态码为Status201Created的HTTP响应
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        #endregion

        #region 删除指定Id的待办事项

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
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
