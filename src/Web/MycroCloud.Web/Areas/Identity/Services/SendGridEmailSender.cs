namespace MicroCloud.Web.Areas.Identity.Services;
public class SendGridEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}