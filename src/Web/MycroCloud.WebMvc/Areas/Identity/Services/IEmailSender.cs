namespace MycroCloud.WebMvc.Areas.Identity.Services {
    public interface IEmailSender {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}