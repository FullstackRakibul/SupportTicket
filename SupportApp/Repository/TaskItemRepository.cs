using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Repository.IReposiroty;
using System.Collections.Immutable;
using System.Linq;

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
                        AssignedTo = taskItemDto.AssignedTo,
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
            //try
            //{
            //    var taskItemData = await _dbcontext.TaskItem.Where(data => data.Status < 5).ToListAsync();
            //    return taskItemData;
            //}
            //catch (Exception ex)
            //{
            //    return Enumerable.Empty<TaskItem>();
            //}


            try
            {
                var taskItemData = await _dbcontext.TaskItem.Where(data => data.Status < 5).ToListAsync();

                // Extract distinct CreatedBy values from taskItemData
                var assignedIds = taskItemData.Select(ti => ti.CreatedBy).Distinct().ToList();

                // Fetch agents whose AgentId matches any of the CreatedBy values
                var engineerData = await _dbcontext.Agent
                    .Where(agent => assignedIds.Contains(agent.EmpCode))
                    .ToListAsync();

                // Map TaskItem data to TaskItemDto and include agent details
                var taskItemDtos = taskItemData.Select(ti => new TaskItemDto
                {
                    Id = ti.Id,
                    TaskItemTitle = ti.TaskItemTitle,
                    AssignedTo = ti.AssignedTo,
                    CreatedBy = ti.CreatedBy,
                    Status = ti.Status,
                    EmpCode = ti.CreatedBy,
                    CreatedByAgentName = engineerData.FirstOrDefault(agent => agent.EmpCode == ti.CreatedBy)?.Name
                }).ToList();

                return taskItemDtos;
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<TaskItem>();
            }
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
        }

        public async Task<TaskItem> TaskItemDetailsInterface(int id)
        {
            try
            {
                var taskItemdata = await _dbcontext.TaskItem.FirstOrDefaultAsync(data=>data.Id==id);
                return taskItemdata;
            }catch(Exception ex)
            {
                throw;
            } 
        }

    }
}
