namespace CYHBrandRepo.Utilities
{
    public interface IMailClient
    {
        Task<bool> SendMailAsync(string toEmail, string subject, string body);
    }
}
