using SupportApp.DTO;
using SupportApp.Helper;

namespace SupportApp.Service
{
    public interface IEmailService
    {
        Task<ApiResponseDto<string>> CreateMailTicket(Mailrequest mailrequest);
    }
}
