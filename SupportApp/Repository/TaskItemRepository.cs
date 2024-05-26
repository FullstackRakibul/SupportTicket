using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Repository.IReposiroty;

namespace SupportApp.Repository
{
    public class TaskItemRepository: ITaskItemInterface
    {
        private SupportAppDbContext _dbcontext;

        public TaskItemRepository(SupportAppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<string> CreateTaskItemInterface(TaskItemDto taskItemDto)
        {
            try
            {
                    var insertTaskItemData = new TaskItem
                    {
                        TaskItemTitle = taskItemDto.TaskItemTitle,
                        AssignedTo = taskItemDto.AssignedTo
                    };
                    _dbcontext.TaskItem.Add(insertTaskItemData);
                    await _dbcontext.SaveChangesAsync();
                    return "Task Create Successfull.";
                

            }
            catch (Exception ex)
            {
                return "repo error";
            }
            
        }

        public async Task<IEnumerable<TaskItem>> GetTaskItemsInterface()
        {
            var taskItemData =  await _dbcontext.TaskItem.ToListAsync();
            return taskItemData;
        }

    }
}
