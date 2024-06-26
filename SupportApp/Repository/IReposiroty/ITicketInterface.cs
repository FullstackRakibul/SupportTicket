﻿using SupportApp.DTO;
using SupportApp.Models;
using static SupportApp.Repository.TicketRepository;
namespace SupportApp.Repository.IReposiroty
{
    public interface ITicketInterface

    {
        Task<IEnumerable<Ticket>> GetAllIssueData();
        Task<ApiResponseDto<IEnumerable<Ticket>>> GetAllIssueDataWithPagination(int take ,int skip);
        Task<ApiResponseDto<Ticket>> GetIssuedata(int issueId);
        Task<string> RaisedIssueWithAttachment(TicketAndTargetDto ticketAndTargetDto);
        Task<string> UpdateRaisedIssueWithAttachment(TicketAndTargetDto ticketAndTargetDto);
        Task<ApiResponseDto<Ticket>> DeleteIssue(int issueId);
        Task<ApiResponseDto<List<Ticket>>> UserCreatedIssueList(string EmpCode);
    }
}
