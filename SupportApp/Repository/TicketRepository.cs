﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportApp.DTO;
using SupportApp.Models;
using SupportApp.Repository.IReposiroty;
using SupportApp.Service;
using System.IdentityModel.Tokens.Jwt;

namespace SupportApp.Repository
{
    public class TicketRepository : ITicketInterface
    {
        private readonly SupportAppDbContext _context;
        private readonly IGlobalFileUploadInterface _globalFileUploadInterface;
        private readonly FromTokenData _fromTokenData;
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


                //var tokenData = await _fromTokenData.tokenDataRetrieve(ticketAndTargetDto.CreatedBy);


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
                    TicketTypeId = ticketAndTargetDto.TicketTypeId ?? 1,
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
                    UnitId = ticketAndTargetDto.UnitId ?? 1,
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
                var ticketDtos = new List<Ticket>();
                foreach (var ticket in issueData)
                {
                    var attachmentUrl = await GetFileDownloadLink(ticket.Id);
                    var ticketDto = new Ticket
                    {
                        Title = ticket.Title,
                        Status = ticket.Status,
                        Description = ticket.Description,
                        Attachment = attachmentUrl,
                        CreatedAt = ticket.CreatedAt,
                        TicketNumber=ticket.TicketNumber

                       
                    };
                    ticketDtos.Add(ticketDto);
                }

                return new ApiResponseDto<IEnumerable<Ticket>> { Data = ticketDtos, Message = "Issue data found", Status = true };

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
                    var issueData = await _context.Ticket.FirstOrDefaultAsync(ticket => ticket.Id == issueId);

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
            if (userIssueData.Count() > 0 && userIssueData != null) {
                return new ApiResponseDto<List<Ticket>> { Status = true, Message = "user created Issue list", Data = userIssueData };
            }
            return new ApiResponseDto<List<Ticket>> { Status = true, Message = "user created Issue list", Data = userIssueData };
        }


        //:::::::::::::::::: Get Raised Ticket List by User
        public async Task<ApiResponseDto<List<Ticket>>> GetRaisedSystemTicketByUser(string EmpCode, int page, int size){
            var userRaisedTicketData = await _context.Ticket
                                       .Where(data=> data.Status<TicketStatus.Deleted && data.IsEmail==false && data.CreatedBy==EmpCode)
                                       .ToListAsync();
            if (userRaisedTicketData != null && userRaisedTicketData.Count() > 0)
            {
                return new ApiResponseDto<List<Ticket>> { Status = true, Message = "user created ticket list", Data = userRaisedTicketData };
            }
            else if(userRaisedTicketData != null)
            {
                return new ApiResponseDto<List<Ticket>> { Status = true, Message = "No Ticket has been raised", Data = null };
            }
            return new ApiResponseDto<List<Ticket>> { Status = false, Message = "Internal error of getting tickets data." , Data= userRaisedTicketData };
        }














        // :::::::::::: private repo 

        public async Task<string> GetFileDownloadLink(int ticketId)
        {
            try
            {
                var fileDownloadLink = await _context.GlobalFileUpload
                    .Where(data => data.TicketId == ticketId)
                    .FirstOrDefaultAsync();

                return fileDownloadLink?.FilePathUrl ?? "null";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<FileResult> DownloadFileAsync(int fileId)
        {
            var filePath = await GetFileDownloadLink(fileId);

            if (filePath == null || !System.IO.File.Exists(filePath))
            {
                return null;
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
            var mimeType = GetMimeType(fileExtension);
            var fileName = Path.GetFileName(filePath);

            return new FileStreamResult(memory, mimeType)
            {
                FileDownloadName = fileName
            };
        }

        private string GetMimeType(string extension)
        {
            switch (extension)
            {
                case ".txt": return "text/plain";
                case ".pdf": return "application/pdf";
                case ".doc": return "application/vnd.ms-word";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".png": return "image/png";
                case ".jpg": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".gif": return "image/gif";
                case ".csv": return "text/csv";
                default: return "application/octet-stream";
            }
        }

    }
}
