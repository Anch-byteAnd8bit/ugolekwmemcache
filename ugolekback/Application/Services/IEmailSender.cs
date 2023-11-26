namespace Ugolek.Backend.Web.Application.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string message, CancellationToken cancellation = default);
}