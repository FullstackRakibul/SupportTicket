using Microsoft.EntityFrameworkCore;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Repository.IReposiroty;

namespace SupportApp.Repository
{
    public class TicketTypeRepository:ITicketTypeInterface
    {
        private readonly SupportAppDbContext _context;

        public TicketTypeRepository(SupportAppDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<TicketType>> GetAllTicketTypeData()
        {
            try
            {
                var allTicketTypeData = await _context.TicketType
                    .Where(data => data.Status ==1).ToListAsync();
                return allTicketTypeData;

            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Enumerable.Empty<TicketType>();
            }
        }

        public async Task<string> CreateTicketTypeInterface(TicketTypeDto ticketTypeDto)
        {
            try
            {
                var newTicketTypeData = new TicketType
                {
                    TypeName = ticketTypeDto.TypeName,
                    Status = 1
                };
                _context.TicketType.Add(newTicketTypeData);
                await _context.SaveChangesAsync();
                return newTicketTypeData.TypeName;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "ticket Type create Failed. check Repo";
            }
        }

        public async Task<TicketType> ShowTicketTypeInterface(int id)
        {
            try
            {
                var ticketTypeData = await _context.TicketType.FirstOrDefaultAsync(x => x.Id == id);
                if(ticketTypeData != null)
                {
                    return ticketTypeData;
                }else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
