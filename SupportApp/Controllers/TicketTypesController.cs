using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Repository.IReposiroty;
using SupportApp.Service;

namespace SupportApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketTypesController : ControllerBase
    {
        private readonly SupportAppDbContext _context;
        private readonly TicketTypeService _ticketTypeService;
        private readonly ITicketTypeInterface _ticketTypeInterface;
       public TicketTypesController(SupportAppDbContext context , TicketTypeService ticketTypeService , ITicketTypeInterface ticketTypeInterface )
        {
            _context = context;
            _ticketTypeService = ticketTypeService;
            _ticketTypeInterface = ticketTypeInterface;
        }

        //::::::::::::::::::: get ticket type data
        [HttpGet]
        [Route("get-all-ticket-type",Name ="getAllTicketType")]
        public async Task<IActionResult> GetTicketType()
        {
            var allTicketTypeData = await _ticketTypeInterface.GetAllTicketTypeData();
            if(allTicketTypeData == null)
            {
                return NotFound();
            }
            return Ok(new ApiResponseDto<IEnumerable<TicketType>>
            {
                Status=true,
                Message="Get All Ticket data",
                Data=allTicketTypeData
            });

        }

        // :::::::::::::::::::::::: Show single Ticket Type.
        [HttpGet()]
        [Route("show-ticket-type-{id}", Name ="showTicketType")]
        public async Task<IActionResult> ShowTicketType(int id)
        {
            var ticketTypeDate =await _ticketTypeInterface.ShowTicketTypeInterface(id);
            if (ticketTypeDate == null)
            {
                return NotFound();
            }
            return Ok(new ApiResponseDto<TicketType>
            {
                Status = true,
                Message = "Ticket Type Show controller request success.",
                Data = ticketTypeDate
            }) ;
        }

        //:::::::::::::::::::: create ticket type api

        [HttpPost]
        [Route("create-ticket-type", Name = "createTicketType")]
        public async Task<IActionResult> TicketTypeCreate(TicketTypeDto ticketTypeDto)
        {
            var craeteTicketTypeData = await _ticketTypeInterface.CreateTicketTypeInterface(ticketTypeDto);
            return Ok(new ApiResponseDto<string>
            {
                Status = true,
                Message = "Ticket Type Create Successfully.",
                Data = craeteTicketTypeData
            });
        }

        // PUT: api/TicketType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicketType(int id, TicketType ticketType)
        {
            if (id != ticketType.Id)
            {
                return BadRequest();
            }

            _context.Entry(ticketType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }




        // DELETE: api/TicketType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketTypeAPI(int id)
        {
            if (_context.TicketType == null)
            {
                return NotFound();
            }
            var ticketType = await _context.TicketType.FindAsync(id);
            if (ticketType == null)
            {
                return NotFound();
            }

            _context.TicketType.Remove(ticketType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TicketTypeExists(int id)
        {
            return (_context.TicketType?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        [HttpGet("ticket/type/list")]
        public async Task<ActionResult<TicketType>> GetTicketTypeList() {
            try
            {
                var getTicketTypeList = await _ticketTypeService.GetTicketTypeListAsync();
                return Ok(getTicketTypeList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }
        }

        [HttpDelete("ticket/type/{id}")]
        public async Task<ActionResult<TicketType>> DeleteTicketType(int id)
        {
            try
            {
                var getTicketTypeList = await _ticketTypeService.DeleteTicketTypeListAsync(id);
                return Ok(getTicketTypeList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }
        }
    }
}
