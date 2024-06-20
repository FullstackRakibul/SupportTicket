using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Service;
using SupportApp.Service.Pagination;
using SupportApp.Repository.IReposiroty;
using SupportApp.Repository;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Diagnostics;

namespace SupportApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly SupportAppDbContext _context;
        private readonly TicketService _ticketService;
        private readonly EmailBoxService _emailBoxService;
        private readonly PaginationService _paginationService;
        private readonly ITicketInterface _ticketInterface;


        public TicketsController(SupportAppDbContext context, TicketService ticketService, EmailBoxService emailBoxService  , PaginationService paginationService , ITicketInterface ticketInterface)
        {
            _context = context;
            _ticketService = ticketService;
            _emailBoxService = emailBoxService;
            _paginationService = paginationService;
            _ticketInterface = ticketInterface;
        }

        // GET: api/Ticket
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Ticket>>> GetTicket()
        //{
        //  try
        //  {
        //      var tickets = await _context.Ticket.Where(ticket => ticket.Status < TicketStatus.Deleted).OrderByDescending(ticket => ticket.CreatedAt)
        //       .ToListAsync();
        //        return tickets;
        //  }
        //  catch (Exception ex)
        //  {
        //      Console.WriteLine(ex);
        //      return StatusCode(500, "Server Response Error.");
        //  }
        //}


        [HttpGet("getTicketFromMail")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketFromMail()
        {
            try
            {
                //var tickets = await _context.Ticket.Where(ticket => ticket.Status != TicketStatus.Deleted && ticket.IsEmail == true).ToListAsync();
                var tickets = await _context.Ticket
               .Where(ticket => ticket.Status != TicketStatus.Deleted && ticket.IsEmail == true)
               .OrderByDescending(ticket => ticket.Status)
               .ToListAsync();
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Server Response Error.");
            }
        }

        // GET: api/Ticket/5

        //[HttpGet("{id}")]
        //public async Task<ActionResult<Ticket>> GetTicket(int id)
        //{
        //  if (_context.Ticket == null)
        //  {
        //      return NotFound();
        //  }
        //    var ticket = await _context.Ticket.FindAsync(id);

        //    if (ticket == null)
        //    {
        //        return NotFound();
        //    }
        //    return ticket;
        //}

        // PUT: api/Ticket/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async  Task<IActionResult> PutTicket(int id, Ticket ticket)
        //{
        //    try
        //    {
        //        if (id != ticket.Id)
        //        {
        //            return NotFound();
        //        }

        //        var ticketdata =await _context.Ticket.FindAsync(id);

        //        if (ticketdata != null) {
        //        ticketdata.Status = ticket.Status;
        //        await _context.SaveChangesAsync();
        //        }



        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }

        //    return null;
        //}



        //[HttpPost]
        //[Route("create-ticket",Name ="createTicket")]
        //public IActionResult CreateTicket([FromBody] TicketAndTargetDto ticketAndTargetDto)
        //{
        //    try
        //    {
        //        _ticketService.CreateTicket(ticketAndTargetDto);
        //        _context.SaveChangesAsync();
        //        return Ok($"Ticket Create Successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        return BadRequest("Create Ticket failed for BadRequest-C");
        //    }
        //}

        
       

        private bool TicketExists(int id)
        {
            return (_context.Ticket?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet("FetchEmailData")]
        public IActionResult FetchEmailDataToDatabase()
        {
            try
            {
                var emailDetailsList = _emailBoxService.GetEmailDetails();
                foreach (var emailDetails in emailDetailsList)
                {
                    _ticketService.CreateTicketFromEmail(emailDetails);
                }
                return Ok(emailDetailsList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        //[HttpPut("id")]
        //public IActionResult UpdateTicketStatus()
        //{
        //    return Ok("update status controller working");
        //}

        //[HttpPost("createTicketWithTarget")]
        //public async Task<ActionResult<Ticket>> createTicketWithTarget([FromBody] TicketAndTargetDto ticketAndTargetDto) {
        //    try {
        //        _ticketService.CreateTicket(ticketAndTargetDto);
        //        return Ok($"Ticket Create Successfully.");

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpGet("ticketStatus")]
        public IActionResult GetTicketStatusEnum()
        {
            var ticketStatusEnumValues = Enum.GetNames(typeof(TicketStatus));
            return Ok(ticketStatusEnumValues);
        }

        [HttpGet("ticketPriority")]
        public IActionResult GetTicketPriorityEnum()
        {
            var ticketPriorityEnumValues = Enum.GetNames(typeof(TicketPriority));
            return Ok(ticketPriorityEnumValues);
        }

        [HttpPut("updateTicketStatus")]
        public async Task<ActionResult<Ticket>> UpdateTicketstatus([FromBody] UpdateTicketStatusDto updateTicketStatusDto )
        {
            try
            {
                _ticketService.UpdateTicketstatus(updateTicketStatusDto);
                return Ok($"Ticket status updated !");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("getTicketDetails")]
        public async Task<ActionResult<Ticket>> GetTicketDetails(int ticketId)
        {
            try
            {
                var ticketDetails = await _context.Ticket
                .Where(t => t.Id == ticketId)
                .FirstOrDefaultAsync();

                if (ticketDetails != null)
                {
                    var reviewDetails = await _context.Review
                        .Where(r => r.TicketId == ticketId)
                        .ToListAsync();


                    Console.WriteLine("Details Data fetched complete !");
                    return Ok(ticketDetails);
                }
                else
                {
                    return BadRequest($"Ticket with ID {ticketId} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Server Response Error.");
            }
        }





		// get ticket list by ticket creator ID
		[HttpGet("getAcknowledgeTicketByCreator/{EmpCode}")]
		public async Task<ActionResult<Ticket>> GetAcknowledgeTicketByCreator(string EmpCode)
		{
			try
			{
                var acknowledgeTicketData = await _ticketService.GetAcknowledgeTicketListByCreatorAsync(EmpCode);
                return Ok(acknowledgeTicketData);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500);
			}
		}

		//------------------------------------------- agent routes apis................


		// get ticket list by ticket creator ID
		[HttpGet("getRecentRaisedTicketByCreator/{EmpCode}")]
		public async Task<ActionResult<Ticket>> GetRecentRaisedTicketByCreator(string EmpCode)
		{
			try
			{
				var acknowledgeTicketData = await _ticketService.GetRecentRaisedTicketListByCreatorAsync(EmpCode);
				return Ok(acknowledgeTicketData);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500);
			}
		}


		// UpdateForCheckTicketStatus API 

		[HttpPost("UpdateForCheckTicketStatus/{ticketId}")]
		public async Task<ActionResult<string>> UpdateForCheckTicketStatus(int ticketId)
		{
			var result = await _ticketService.UpdateForCheckTicketStatus(ticketId);
			return Ok(result);
		}

		//// Pagination API
		//[HttpGet("getPaginationList/{Skip}/{Take}")]
  //      public IActionResult GetPaginationList(int Skip, int Take)
  //      {

		//	try
		//	{
		//		var tickets = _ticketService.GetPaginationList(Skip, Take);
		//		return Ok(tickets);
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine(ex);
		//		return StatusCode(500, "Server Response Error.");
		//	}
		//}


        // Email List API
		[HttpGet("GetMailTicketList/{Skip}/{Take}")]
		public IActionResult GetMailTicketList(int Skip, int Take)
		{
			try
			{
                var getMailTicketList = _ticketService.GetMailTicketList(Skip, Take);
                return Ok(getMailTicketList);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500, "Server Response Error.");
			}
		}

        [HttpPost("soft-reminder/{ticketId}")]
        public async Task<IActionResult> SoftReminder(int ticketId)
        {
            try
            {
                var reminder = await _ticketService.Softreminder(ticketId);
                return Ok(reminder);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Server Response Error.");
            }
        }




        //::::::::::::::::::::::::::::::::::::::  INTERFACE AND REPOSITORY :::::::::::::::::::::::::::::::::::::::






        //::::::::::::::::::::::::::::::::: Get all issue data 

        [HttpGet]
        [Route("get-all-issue-data" , Name ="GetAllIssueData")]
        public async Task<ActionResult> GetTicket()
        {
            try
            {
                var tickets = await _ticketInterface.GetAllIssueData();
                return Ok(new ApiResponseDto<IEnumerable<Ticket>>
                {
                    Status = true,
                    Message = "Request response for get all issue data.",
                    Data = tickets
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Server Response Error.");
            }
        }


        //::::::::::::::::::::::::::::::::: Get all issues pagination data 

        [HttpGet]
        [Route("get-all-issue-data-with-pagination")]
        public async Task<IActionResult> GetTicketWithPagination(int take , int skip)
        {
            try
            {
                var ticketPaginationData = await _ticketInterface.GetAllIssueDataWithPagination(take , skip);
                return Ok(new ApiResponseDto<IEnumerable<Ticket>>
                {
                    Status = ticketPaginationData.Status,
                    Message = ticketPaginationData.Message,
                    Data = ticketPaginationData.Data
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Server Response Error.");
            }
        }


        //::::::::::::::::::::::::::::::::: Get single issues data

        [HttpGet]
        [Route("get-issue-data")]
        public async Task<IActionResult> GetIssueData(int issueId)
        {
            try
            {
                var issueData = await _ticketInterface.GetIssuedata(issueId);
                return Ok(new ApiResponseDto<Ticket>
                {
                    Status = issueData.Status,
                    Message = issueData.Message,
                    Data = issueData.Data
                });

            }catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return StatusCode(500, "Server Error");
            }
        }


        //:::::::::::::::::::::::::::::::: Raised Issue
        [HttpPost]
        [Route("raised-issue", Name = "raisedIssueController")]
        public async Task<ActionResult> RaisedIssueWithAttachment([FromForm] TicketAndTargetDto ticketAndTargetDto)
        {
            try
            {
                Debug.WriteLine(ticketAndTargetDto);

                var responseData = await _ticketInterface.RaisedIssueWithAttachment(ticketAndTargetDto);

                return Ok(new ApiResponseDto<string>
                {
                    Status = true,
                    Message = "Request successfull.",
                    Data = responseData
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //::::::::::::::::::::::::::::::::: Update Issue
        //[HttpPut("updateIssue1")]
        //[Route("update-attachment-issue", Name = "updateIssueController")]
        //public async Task<IActionResult> UpdateIssue(TicketAndTargetDto ticketAndTargetDto)
        //{
        //    var updateIssueData = await _ticketInterface.UpdateRaisedIssueWithAttachment(ticketAndTargetDto);

        //    return Ok(new ApiResponseDto<string>
        //    {
        //        Status = true,
        //        Message = "Issue Data Updated Successfully .",
        //        Data = updateIssueData
        //    });
        //}

        // ::::::::::::::::::::::::::: Delete issue 

        //[HttpDelete("deleteIssue")]
        //[Route("delete-issue")]
        //public async Task<IActionResult> DeleteTicket(int issueId)
        //{
        //    try
        //    {
        //        if (issueId != null && issueId > 0)
        //        {
        //            var response = await _ticketInterface.DeleteIssue(issueId);
        //            return Ok(new ApiResponseDto<Ticket> { Status = true, Message = response.Message, Data = response.Data });
        //        }
        //        else
        //        {
        //            return Ok(new ApiResponseDto<string> { Status = true, Message = "Delete request response.", Data = "Issue not found." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Server Response Error.");
        //    }

        //}



        // userwise ticket 

        //[HttpGet("getIssueByCreator")]
        //[Route("user-created-issue", Name = "createdByUser")]
        //public async Task<IActionResult> GetIssueByCreator(string EmpCode)
        //{
        //    try
        //    {
        //        var userCreatedIssueData = await _ticketInterface.UserCreatedIssueList(EmpCode);
        //        if (userCreatedIssueData != null)
        //        {
        //            return Ok(new ApiResponseDto<List<Ticket>> { Status = userCreatedIssueData.Status, Message = userCreatedIssueData.Message, Data = userCreatedIssueData.Data });
        //        }
        //        return Ok(new ApiResponseDto<List<Ticket>> { Status = userCreatedIssueData.Status, Message = userCreatedIssueData.Message, Data = userCreatedIssueData.Data });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Server Error");
        //    }


        //}
    }
}
