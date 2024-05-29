using SupportApp.DTO;
using SupportApp.Models;

namespace SupportApp.Repository.IReposiroty
{
    public interface ITaskItemInterface
    {
        Task<TaskItemDto> CreateTaskItemInterface(TaskItemDto taskItemDto);
        Task<IEnumerable<TaskItemDto>> GetTaskItemsInterface();
        Task<string> MarkTaskAsDoneInterface(int id);
        Task<string> UpdateTaskItemStatusInterface(int id);

        Task<TaskItemDto> TaskItemDetailsInterface(int id);
    }
}
