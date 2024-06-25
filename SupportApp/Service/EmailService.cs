using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using SupportApp.Helper;
using Microsoft.Extensions.Options;
using SupportApp.DTO;
using MailKit;

namespace SupportApp.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options) { 
            this._emailSettings = options.Value;
        }

        public async Task<ApiResponseDto<string>> CreateMailTicket(Mailrequest mailrequest) {
            try
            {

                var toMail = "rakibul.it@hameemgroup.com";

                var email = new MimeMessage();
                //using mimekit to sent the mail 
                email.Sender = MailboxAddress.Parse(_emailSettings.Email);
                //email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
                email.To.Add(MailboxAddress.Parse(toMail));
                email.Subject = mailrequest.Subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = mailrequest.Body;
                email.Body = builder.ToMessageBody();

                //create email client
                using var smtp = new SmtpClient();
                smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.SslOnConnect);
                smtp.Authenticate(_emailSettings.Email, _emailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return new ApiResponseDto<string> { Status = false, Message = "Create Ticket Successfully.", Data = email.MessageId.ToString() };

            }
            catch (Exception ex) {
                return new ApiResponseDto<string> { Status = false, Message = "try exception raised", Data = ex.Message.ToString() };
            }
        }
    }
}
