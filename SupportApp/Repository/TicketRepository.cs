using Microsoft.EntityFrameworkCore;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Repository.IReposiroty;

namespace SupportApp.Repository
{
    public class TicketRepository : ITicketInterface
    {
        private readonly SupportAppDbContext _context;
        private readonly IGlobalFileUploadInterface _globalFileUploadInterface;
        public TicketRepository(SupportAppDbContext context, IGlobalFileUploadInterface globalFileUploadInterface)
        {
            _context = context;
            _globalFileUploadInterface = globalFileUploadInterface;
        }

        //::::::::::::::::::::::::::::: Get All Issue data 

        public async Task<IEnumerable<Ticket>> GetAllIssueData()
        {
            try
            {
                var allDataFromIssueList = await _context.Ticket
                    .OrderByDescending(data => data.CreatedAt)
                    .Where(data => data.Status < TicketStatus.Deleted && data.IsEmail == false)
                    .ToListAsync();


                return allDataFromIssueList ?? new List<Ticket>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new List<Ticket>();
            }
        }

        // :::::::::::::::::::::::::::::  Create Ticket 
        public async Task<string> RaisedIssueWithAttachment(TicketAndTargetDto ticketAndTargetDto)
        {
            try
            {
                var generatedTicketNumber = GenerateTicketNumber();
                var raisedIssueData = new Ticket
                {
                    Title = ticketAndTargetDto.Title,
                    TicketNumber = generatedTicketNumber,
                    Description = ticketAndTargetDto.Description,
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    CreatedBy = ticketAndTargetDto.CreatedBy,
                    MessageId = generatedTicketNumber,
                    Priority = TicketPriority.Regular,
                    Status = TicketStatus.Open,
                    IsEmail = false,
                    TicketTypeId = ticketAndTargetDto.TicketTypeId,
                    UpdatedAt = null,

                };

                _context.Ticket.Add(raisedIssueData);
                await _context.SaveChangesAsync();

                //:::::::::::::::::::::: global File Upload

                if (ticketAndTargetDto.Attachment != null)
                {
                    var globalFileUploadData = await _globalFileUploadInterface.UploadFile(new GlobalFileUploadDto
                    {
                        TicketId = raisedIssueData.Id,
                        FolderIndex = "supportTicket",
                        UploadedFile = ticketAndTargetDto.Attachment,
                        FilePathUrl = ""
                    });

                    raisedIssueData.Attachment = globalFileUploadData.Id.ToString();
                }
                // Update ticket with attachment path
                _context.Ticket.Update(raisedIssueData);
                await _context.SaveChangesAsync();
                //::::::::::::::::::::::

                var assignTargetData = new Target
                {
                    TicketId = raisedIssueData.Id,
                    DepartmentId = ticketAndTargetDto.DepartmentId,
                    UnitId = ticketAndTargetDto.UnitId,
                };
                _context.Target.Add(assignTargetData);
                await _context.SaveChangesAsync();

                return "Issue Raised Successfully";

            }
            catch (Exception ex)
            {
                return ("Issue raised failed.");
            }
        }


        // :::::::::::::::::::::  Generate Ticket Number 

        public string GenerateTicketNumber()
        {
            string ticketNumber;
            bool isUnique = false;
            do
            {
                // Generate a new ticket number
                ticketNumber = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999);
                // Check if the ticket number exists in the database
                isUnique = CheckIfTicketNumberExists(ticketNumber);
            }
            while (!isUnique);
            return ticketNumber;
        }

        private bool CheckIfTicketNumberExists(string ticketNumber)
        {
            var existingTicket = _context.Ticket.FirstOrDefault(t => t.TicketNumber == ticketNumber);
            return existingTicket == null;

        }


        // :::::::::::::::::::::::::: Update Ticket 
        public async Task<string> UpdateRaisedIssueWithAttachment(TicketAndTargetDto ticketAndTargetDto)
        {
            var retrieveData = await _context.Ticket.FirstOrDefaultAsync(t => t.Id == ticketAndTargetDto.TicketId);
            if (retrieveData != null)
            {
                return "Ticket Data Not Exits.";
            }

            return "This is a update issue test.";
        }


        // ::::::::::::::::::  get all issue data form pagination
        public async Task<ApiResponseDto<IEnumerable<Ticket>>> GetAllIssueDataWithPagination(int take, int skip)
        {
            try
            {
                if (take > 0 && skip > 0)
                {
                    var issueData = await _context.Ticket
                        .OrderByDescending(data => data.CreatedAt)
                        .Take(take)
                        .Skip(skip)
                        .ToArrayAsync();
                    return new ApiResponseDto<IEnumerable<Ticket>> { Data = issueData, Message = "Issue data found", Status = false };
                }
                else
                {
                    var issueData = await _context.Ticket
                        .OrderByDescending(data => data.CreatedAt)
                        .Take(take)
                        .Skip(0)
                        .ToArrayAsync();
                    return new ApiResponseDto<IEnumerable<Ticket>> { Data = issueData, Message = "Issue data found", Status = false };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new ApiResponseDto<IEnumerable<Ticket>> { Data = null, Message = "Issue Id Invalid", Status = false };
            }
        }
        //:::::::::::::::::::: get single issue data 
        public async Task<ApiResponseDto<Ticket>> GetIssuedata(int issueId)
        {
            try
            {
                if (issueId != null)
                {
                    var issueData = await _context.Ticket.FirstOrDefaultAsync(issue => issue.Id == issueId);
                    return new ApiResponseDto<Ticket> { Data = issueData, Message = "Issue Data found", Status = true };
                }
                else
                {
                    return new ApiResponseDto<Ticket> { Data = null, Message = "Issue Data not found", Status = true };
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new ApiResponseDto<Ticket> { Data = null, Message = "Issue Id Invalid", Status = false };
            }
        }


        //::::::::::::::::::::: delete issue data 

        public async Task<ApiResponseDto<Ticket>> DeleteIssue(int issueId)
        {
            try
            {
                if (issueId != null)
                {
                    var issueData = await _context.Ticket.FirstOrDefaultAsync(ticket => ticket.Id==issueId);
                    
                    issueData.Status = TicketStatus.Deleted;
                    await _context.SaveChangesAsync();
                    return new ApiResponseDto<Ticket> { Data = issueData, Message = "Deleted Successfully", Status = true };
                }
                else
                {
                    return new ApiResponseDto<Ticket> { Data = null, Message = "Issue data null", Status = true };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Ticket> { Status = false, Message = "Delete operation invalid.", Data = null };
            }
            
        }
    }
}
