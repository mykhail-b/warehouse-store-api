using ClassLibrary;
using ClassLibrary.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Backend.Services.Infrastructure;

public interface IMailService
{
    Task SendOrderConfirmationEmailAsync(OrderConfirmationMail mail);
    Task SendOrderShippingMailAsync(OrderShippingMail mail);
    Task SendRegisterEmailConfirmationAsync(RegisterConfirmationMail mail);
}

public class MailService : IMailService
{
    private readonly EmailSettings _settings;

    public MailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendOrderConfirmationEmailAsync(OrderConfirmationMail mail)
    {
        var body = $"""
            Hi {mail.RecipientName},

            Your order #{mail.OrderId} has been confirmed!

            Items:
            {string.Join("\n", mail.Items.Select(i => $"- {i.Name} x{i.Quantity} — {i.Price * i.Quantity:C}"))}

            Total: {mail.TotalPrice:C}
            Shipping to: {mail.ShippingAddress}

            Thank you for your order!
            Computer Store
            """;

        mail.Subject = $"Order #{mail.OrderId} Confirmed";
        mail.Body = body;

        await SendAsync(mail);
    }

    public async Task SendOrderShippingMailAsync(OrderShippingMail mail)
    {
        mail.Subject = $"Order #{mail.OrderId} Has Been Shipped";
        mail.Body = $"""
            Hi {mail.RecipientName},

            Your order #{mail.OrderId} has been shipped!
            Shipping number: {mail.ShippingNumber}

            Thank you for shopping with us!
            Computer Store
            """;

        await SendAsync(mail);
    }

    private async Task SendAsync(MailTemplate mail)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(mail.To));
        message.Subject = mail.Subject;
        message.Body = new TextPart("plain") { Text = mail.Body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.None);

        if (!string.IsNullOrEmpty(_settings.Username))
            await client.AuthenticateAsync(_settings.Username, _settings.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendRegisterEmailConfirmationAsync(RegisterConfirmationMail mail)
    {
        mail.Subject = "Confirm your email address";
        mail.Body = $"""
        Hi {mail.RecipientName},

        Please confirm your email address by clicking the link below:

        {mail.ConfirmationLink}

        If you didn't register, you can ignore this email.

        Computer Store
        """;

        await SendAsync(mail);
    }
}