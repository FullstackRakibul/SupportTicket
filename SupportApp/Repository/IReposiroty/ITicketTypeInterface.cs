using SupportApp.DTO;
using SupportApp.Models;

namespace SupportApp.Repository.IReposiroty
{
    public interface ITicketTypeInterface
    {
        Task<IEnumerable<TicketType>> GetAllTicketTypeData();
        Task<string> CreateTicketTypeInterface(TicketTypeDto ticketTypeDto);
        Task<TicketType> ShowTicketTypeInterface(int id);
    }
}
