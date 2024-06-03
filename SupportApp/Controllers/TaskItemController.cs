using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Repository.IReposiroty;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SupportApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemController : ControllerBase
    {
        private ITaskItemInterface _taskItemInterface;
        public TaskItemController(ITaskItemInterface taskItemInterface) {
        
            _taskItemInterface = taskItemInterface;
        }


        [HttpGet]
        [Route("all-task-item-list")]
        public async Task<IActionResult> GellAllTaskItemList()
        {
            var itemData = await _taskItemInterface.GetTaskItemsInterface();
            if (itemData == null)
            {
                return NotFound();
            }

            return Ok(new ApiResponseDto<IEnumerable<TaskItemDto>>
            {
                Status = true,
                Message = "Task create request response :",
                Data = itemData
            });
        }

        [HttpGet()]
        [Route("item-details-{id}",Name = "getTaskItemDetails")]
        public async Task<IActionResult> GetTaskItemDetails(int id)
        {
            var itemDetailsData =await _taskItemInterface.TaskItemDetailsInterface(id);
            return Ok(new ApiResponseDto<TaskItemDto>
            {
                Status = true,
                Message = "item details request response :",
                Data = itemDetailsData
            });
        }

        [HttpPost]
        [Route("create-task-item",Name ="createTaskItem")]
        public async Task<IActionResult> CreateTaskItem(TaskItemDto taskItemDto)
        {
            var itemData = await _taskItemInterface.CreateTaskItemInterface(taskItemDto);
            return Ok(new ApiResponseDto<TaskItemDto>
            {
                Status=true,
                Message="Task create request response :",
                Data= itemData
            });
        }

        
        [HttpPut()]
        [Route("mark-done-{id}",Name ="markDone")]
        public async Task<IActionResult> MarkDoneTaskItem(int id)
        {
            var markData = await _taskItemInterface.MarkTaskAsDoneInterface(id);

            return Ok(new ApiResponseDto<string>
            {
                Status = true,
                Message = "Response from mark done",
                Data = markData
            });
        }

        [HttpPut()]
        [Route("update-status-{id}", Name = "updateStatus")]
        public async Task<IActionResult> UpdateStatusTaskItem(int id)
        {
            var markData = await _taskItemInterface.UpdateTaskItemStatusInterface(id);

            return Ok(new ApiResponseDto<string>
            {
                Status = true,
                Message = "Response from update status",
                Data = markData
            });
        }

        //// DELETE api/<ValuesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
