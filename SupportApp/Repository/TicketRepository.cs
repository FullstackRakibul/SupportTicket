using Microsoft.EntityFrameworkCore;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Repository.IReposiroty;
using System.IdentityModel.Tokens.Jwt;

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


                var tokenData = await tokenDataRetrieve(ticketAndTargetDto.CreatedBy);


                var raisedIssueData = new Ticket
                {
                    Title = ticketAndTargetDto.Title,
                    TicketNumber = generatedTicketNumber,
                    Description = ticketAndTargetDto.Description,
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    CreatedBy = tokenData,
                    MessageId = generatedTicketNumber,
                    Priority = TicketPriority.Regular,
                    Status = TicketStatus.Open,
                    IsEmail = false,
                    TicketTypeId = ticketAndTargetDto.TicketTypeId??1,
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
                    DepartmentId = ticketAndTargetDto.DepartmentId ?? 1,
                    UnitId = ticketAndTargetDto.UnitId??1,
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

        public async Task<ApiResponseDto<IEnumerable<Ticket>>> GetAllIssueDataWithPagination(int page, int size)
        {
            try
            {
                page = page > 0 ? page : 1;
                size = size > 0 ? size : 10;
                int skip = (page - 1) * size;
                int take = size;

                var issueData = await _context.Ticket
                        .Where(data => data.Status < TicketStatus.Deleted && data.IsEmail == false)
                        .OrderByDescending(data => data.CreatedAt)
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();

                foreach (var item in issueData)
                {
                    item.Attachment = await GetFileDownloadLink(item.Id);
                }

                return new ApiResponseDto<IEnumerable<Ticket>> { Data = issueData, Message = "Issue data found", Status = true };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new ApiResponseDto<IEnumerable<Ticket>> { Data = null, Message = "Issue Id Invalid", Status = false };
            }
        }


        // ::::::::::::::::::  get all mail issue data form pagination

        public async Task<ApiResponseDto<IEnumerable<Ticket>>> GetAllMailIssueDataWithPagination(int page, int size)
        {
            try
            {
                page = page > 0 ? page : 1;
                size = size > 0 ? size : 10;
                int skip = (page - 1) * size;
                int take = size;

                var issueData = await _context.Ticket
                        .Where(data => data.Status < TicketStatus.Deleted && data.IsEmail == true)
                        .OrderByDescending(data => data.CreatedAt)
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
                return new ApiResponseDto<IEnumerable<Ticket>> { Data = issueData, Message = "Issue data found", Status = true };

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

        //:::::::::::::::::  user Created Issue List

        public async Task<ApiResponseDto<List<Ticket>>> UserCreatedIssueList(string EmpCode)
        {
            var userIssueData = await _context.Ticket.Where(data => data.CreatedBy == EmpCode).ToListAsync();
            if (userIssueData.Count() > 0  && userIssueData !=null) {
                return new ApiResponseDto<List<Ticket>> { Status = true, Message = "user created Issue list", Data = userIssueData };

            }
            return new ApiResponseDto<List<Ticket>> { Status = true, Message = "user created Issue list", Data = userIssueData };
        }

























        // :::::::::::: private repo 

        public async Task<string> GetFileDownloadLink(int ticketId)
        {
            try
            {
                var fileDownloadLink = _context.GlobalFileUpload
                    .Where(data=> data.TicketId == ticketId)
                    .FirstOrDefault();

                return fileDownloadLink.FilePathUrl?? "null";

            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // ::::::::::::: jwt token data Retrieve

        public async Task<string> tokenDataRetrieve( string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Token cannot be null or empty", nameof(token));
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                throw new ArgumentException("Invalid token format", nameof(token));
            }

            var claims = jsonToken.Claims;
            var empCode = claims.FirstOrDefault(claim => claim.Type == "EmpCode")?.Value;

            if (string.IsNullOrEmpty(empCode))
            {
                throw new Exception("EmpCode claim not found in token");
            }

            return await Task.FromResult(empCode);
        }
    }
}
