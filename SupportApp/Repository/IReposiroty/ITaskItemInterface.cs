using SupportApp.DTO;
using SupportApp.Models;

namespace SupportApp.Repository.IReposiroty
{
    public interface ITaskItemInterface
    {
        Task<string> CreateTaskItemInterface(TaskItemDto taskItemDto);
        Task<IEnumerable<TaskItem>> GetTaskItemsInterface();
        Task<string> MarkTaskAsDoneInterface(int id);

        Task<TaskItem> TaskItemDetailsInterface(int id);
    }
}
