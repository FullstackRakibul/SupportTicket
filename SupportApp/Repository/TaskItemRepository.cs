using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
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
                        AssignedTo = taskItemDto.AssignedTo.ToString(),
                        CreatedAt=DateTime.Now,
                        CreatedBy=taskItemDto.CreatedBy,
                        Status= 0 
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
            var taskItemData =  await _dbcontext.TaskItem.Where(data=>data.Status <5).ToListAsync();
            return taskItemData;
        }

        public async Task<string> MarkTaskAsDoneInterface(int id)
        {
            try
            {
                var makeMark= await _dbcontext.TaskItem.FirstOrDefaultAsync(data => data.Id==id);
                if (makeMark != null)
                {
                    makeMark.Status= 5;
                }
                await _dbcontext.SaveChangesAsync();
                return "Task item status update.";
            }
            catch(Exception ex) { }
            {
                return "Operation failed !";
            }

            return id.ToString();
        }

    }
}
