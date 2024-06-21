using SupportApp.DTO;
using SupportApp.Models;
using static SupportApp.Repository.TicketRepository;
namespace SupportApp.Repository.IReposiroty
{
    public interface ITicketInterface

    {
        Task<IEnumerable<Ticket>> GetAllIssueData();
        Task<ApiResponseDto<IEnumerable<Ticket>>> GetAllIssueDataWithPagination(int page ,int size);
        Task<ApiResponseDto<Ticket>> GetIssuedata(int issueId);
        Task<string> RaisedIssueWithAttachment(TicketAndTargetDto ticketAndTargetDto);
        Task<string> UpdateRaisedIssueWithAttachment(TicketAndTargetDto ticketAndTargetDto);
        Task<ApiResponseDto<Ticket>> DeleteIssue(int issueId);
        Task<ApiResponseDto<List<Ticket>>> UserCreatedIssueList(string EmpCode);
    }
}
//if (take > 0 && skip >= 0)
//{
//    var issueData = await _context.Ticket
//        .OrderByDescending(data => data.CreatedAt)
//        .Take(take)
//        .Skip(skip)
//        .ToArrayAsync();
//    return new ApiResponseDto<IEnumerable<Ticket>> { Data = issueData, Message = "Issue data found", Status = false };
//}
//else
//{
//    var issueData = await _context.Ticket
//        .OrderByDescending(data => data.CreatedAt)
//        .Take(take)
//        .Skip(0)
//        .ToArrayAsync();
//    return new ApiResponseDto<IEnumerable<Ticket>> { Data = issueData, Message = "Issue data found", Status = false };
//}