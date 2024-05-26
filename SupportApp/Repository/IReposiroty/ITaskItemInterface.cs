using SupportApp.DTO;
using SupportApp.Models;

namespace SupportApp.Repository.IReposiroty
{
    public interface ITaskItemInterface
    {
        Task<string> CreateTaskItemInterface(TaskItemDto taskItemDto);
        Task<IEnumerable<TaskItem>> GetTaskItemsInterface();
    }
}
