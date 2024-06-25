using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using SupportApp.DTO;
using SupportApp.Helper;
using SupportApp.Service;

namespace SupportApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly EmailBoxService _emailBoxService;


        public EmailsController(IEmailService service , EmailBoxService emailBoxService)
        {
            _emailService = service;
            _emailBoxService = emailBoxService;
        }

        [HttpPost("SendMail")]
        public async Task<IActionResult> SendMail() {
            try {
                var mailrequest = new Mailrequest();
                mailrequest.ToEmail = "it@dhakawestern.com";
                mailrequest.Subject = "Send mail test";
                mailrequest.Body = "This is a test mail body for fetch data";
                if (_emailService != null)
                {
                    await _emailService.CreateMailTicket(mailrequest);
                    return Ok(new ApiResponseDto<string>
                    {
                        Status = true,
                        Message = "Send Test Mail Success.",
                        Data = mailrequest.ToJson()
                    });
                }
                else {
                    return BadRequest("A Bad request");
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                return StatusCode(500);
            }
        }
        
        [HttpPost("create-mail-ticket")]
        public async Task<ActionResult> ComposeMailTicket([FromForm] Mailrequest mailRequest)
        {
            try
            {
                if (mailRequest != null && _emailService != null)
                {
                    var sendMailResponse = await _emailService.CreateMailTicket(mailRequest);
                    return Ok(new ApiResponseDto<string>
                    {
                        Status= true,
                        Message = sendMailResponse.Message,
                        Data = sendMailResponse.Data,
                    });
                }
                else
                {
                    return BadRequest("Mail send failed !!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetMails")]
        public IActionResult GetMails()
        {
            try
            {
                var emailDetailsList = _emailBoxService.GetEmailDetails();
                return Ok(emailDetailsList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
