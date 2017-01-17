using MailKit.Net.Smtp;
using MimeKit;

namespace CandyShop.Api.Infra.Email
{
    public class Mail
    {
        public static void Send(string to, string subject, string body)
        {
            using (var smtp = new SmtpClient())
            {
                smtp.Connect("mail.engsolutions.com.br", 587, false);
                smtp.Authenticate("gcproteste@engsolutions.com.br", "123321");
                smtp.Send(new MimeMessage
                {
                    Subject = subject,
                    Body = new TextPart("plain") { Text = body },
                    From = { new MailboxAddress("gcproteste@engsolutions.com.br") },
                    To = { new MailboxAddress(to) },
                    Priority = MessagePriority.Urgent
                });
                smtp.Disconnect(true);
            }
        }
    }
}
