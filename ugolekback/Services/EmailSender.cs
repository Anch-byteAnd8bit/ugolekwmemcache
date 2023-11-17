using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace ugolekback.EmailF;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string message, CancellationToken cancellation = default);
}

public class EmailSender : IEmailSender {
    private readonly EmailServiceOptions options;

    public EmailSender(IOptions<EmailServiceOptions> options) {
        this.options = options.Value;
    }

    public async Task SendEmailAsync(string email, string message, CancellationToken cancellation) {
        using var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("Подтверждение заказа", "anch@ya.ru"));

        emailMessage.To.Add(new MailboxAddress("", email));

        const string mailSubject = "Подтвердите код";
        
        emailMessage.Subject = mailSubject;

        emailMessage.Body = new TextPart(TextFormat.Html) {
            Text = message
        };

        using var client = await BuildUnderlyingClientAsync(cancellation);

        var response = await client.SendAsync(emailMessage, cancellation);

        await client.DisconnectAsync(true, cancellation);
    }
    
    private async Task<SmtpClient> BuildUnderlyingClientAsync(CancellationToken c) {
        var client = new SmtpClient {
            DeliveryStatusNotificationType = DeliveryStatusNotificationType.Full
        };

        try {
            await client.ConnectAsync(options.Host, options.Port, false, cancellationToken: c);

            return client;
        } catch {
            client.Dispose();

            throw;
        }
    }
}