using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Repository.IReposiroty;
using System.Linq;

namespace SupportApp.Repository
{
    public class TaskItemRepository: ITaskItemInterface
    {
        private readonly SupportAppDbContext _dbcontext;

        public TaskItemRepository(SupportAppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<TaskItemDto> CreateTaskItemInterface(TaskItemDto taskItemDto)
        {
            try
            {
                    var insertTaskItemData = new TaskItem
                    {
                        TaskItemTitle = taskItemDto.TaskItemTitle,
                        AssignedTo = taskItemDto.AssignedTo,
                        CreatedAt=DateTime.Now,
                        CreatedBy= string.IsNullOrEmpty(taskItemDto.CreatedBy) ? "SERVER" : taskItemDto.CreatedBy,
                        Status = 0 
                    };
                    _dbcontext.TaskItem.Add(insertTaskItemData);
                    await _dbcontext.SaveChangesAsync();

                var createdTaskItemDto = new TaskItemDto
                {
                    Id = insertTaskItemData.Id,
                    TaskItemTitle = insertTaskItemData.TaskItemTitle,
                    CreatedAt = insertTaskItemData.CreatedAt,
                    CreatedByAgentName = await GetAgentNameByEmpCode(insertTaskItemData.CreatedBy),
                    AssignToAgentName  = await GetAgentNameByEmpCode(insertTaskItemData.AssignedTo),
                    Status = insertTaskItemData.Status,
                };

                return createdTaskItemDto;

            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        public async Task<IEnumerable<TaskItemDto>> GetTaskItemsInterface()
        {
            var taskItems = await _dbcontext.TaskItem
            .Where(data => data.Status < 5)
            .OrderByDescending(data => data.CreatedAt)
            .ToListAsync();

            var taskItemDtos = new List<TaskItemDto>();
            foreach (var taskItem in taskItems)
            {
                var taskItemDto = new TaskItemDto
                {
                    Id = taskItem.Id,
                    TaskItemTitle = taskItem.TaskItemTitle,
                    CreatedAt = taskItem.CreatedAt,
                    AssignToAgentName = await GetAgentNameByEmpCode(taskItem.AssignedTo),
                    CreatedByAgentName = await GetAgentNameByEmpCode(taskItem.CreatedBy),
                    Status= taskItem.Status,
                };
                taskItemDtos.Add(taskItemDto);
            }
            return taskItemDtos;
        }

        public async Task<TaskItemDto> TaskItemDetailsInterface(int id)
        {
            try
            {
                var taskItem = await _dbcontext.TaskItem.FirstOrDefaultAsync(data => data.Id == id);
                if (taskItem == null)
                {
                    return null; 
                }

                var taskItemDto = new TaskItemDto
                {
                    Id = taskItem.Id,
                    TaskItemTitle = taskItem.TaskItemTitle,
                    CreatedAt = taskItem.CreatedAt,
                    Status = taskItem.Status,
                    CreatedByAgentName = await GetAgentNameByEmpCode(taskItem.CreatedBy),
                    AssignToAgentName = await GetAgentNameByEmpCode(taskItem.AssignedTo)
                };

                return taskItemDto;
            }
            catch(Exception ex)
            {
                throw;
            } 
        }



        private async Task<string> GetAgentNameByEmpCode(string empCode)
        {
            var agentName = await _dbcontext.Agent
            .Where(agent => agent.EmpCode == empCode)
            .Select(agent => agent.Name)
            .FirstOrDefaultAsync();
            return agentName ?? "Not Define";
        }


        public async Task<string> MarkTaskAsDoneInterface(int id)
        {
            try
            {
                var makeMark = await _dbcontext.TaskItem.FirstOrDefaultAsync(data => data.Id == id);
                if (makeMark != null)
                {
                    makeMark.Status = 5;
                    makeMark.UpdatedAt = DateTime.Now;
                }
                await _dbcontext.SaveChangesAsync();
                return "Task item status update.";
            }
            catch (Exception ex) { }
            {
                return "Operation failed !";
            }
        }


        public async Task<string> UpdateTaskItemStatusInterface(int id)
        {
            try
            {
                var makeMark = await _dbcontext.TaskItem.FirstOrDefaultAsync(data => data.Id == id);
                if (makeMark != null)
                {
                    makeMark.Status += 1;
                    makeMark.UpdatedAt = DateTime.Now;
                }
                await _dbcontext.SaveChangesAsync();
                return "Task item status update.";
            }
            catch (Exception ex) { }
            {
                return "Operation failed !";
            }
        }

    }
}
