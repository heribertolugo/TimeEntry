using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using TimeClock.Core.Models;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.JdeSync")]
namespace TimeClock.Core.Helpers;

// this class and how to get its hash to determine if its new item - that entire logic is goofy. and i'm sorry
internal class Emailer
{
    public static async Task<EmailContent> SendMissedPunch(string to, MissedPunchEmailData emailData, bool isFirstTimeEmail, EmailConnectivity emailConnectivity, string[]? bcc = null, CancellationToken cancellationToken = default)
    {
        EmailContent emailContent = new(to, emailData);

        await SendMailToHostAccount(emailContent.To, $"{(isFirstTimeEmail ? string.Empty : "REMINDER: ")}{emailContent.Subject}", emailContent.Message, Enumerable.Empty<string>(), emailConnectivity, bcc, cancellationToken);

        return emailContent;
    }
    public static async Task<EmailContent> SendTimeEntryRequest(string to, PunchEntryEmailData emailData, bool isFirstTimeEmail, EmailConnectivity emailConnectivity, string[]? bcc = null, CancellationToken cancellationToken = default)
    {
        EmailContent emailContent = new(to, emailData);

        await SendMailToHostAccount(emailContent.To, $"{(isFirstTimeEmail ? string.Empty : "REMINDER: ")}{emailContent.Subject}", emailContent.Message, Enumerable.Empty<string>(), emailConnectivity, bcc, cancellationToken);

        return emailContent;
    }

    public static EmailContent GetMissedPunchEmailContent(string to, MissedPunchEmailData emailData)
    {
        return new EmailContent(to, emailData);
    }

    public static EmailContent GetTimeEntryRequestEmailContent(string to, PunchEntryEmailData emailData)
    {
        return new EmailContent(to, emailData);
    }

    private static async Task SendMailToHostAccount(string to, string subject, string message, IEnumerable<string> attachments, EmailConnectivity emailConnectivity, string[]? bcc = null, CancellationToken cancellationToken = default)
    {
        bcc ??= [];

        try
        {
            using (MailMessage mail = new())
            {
                SmtpClient SmtpServer = new(emailConnectivity.Server);

                mail.From = new MailAddress(emailConnectivity.Email);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = message;

                foreach (string bc in bcc)
                    mail.Bcc.Add(bc);

                SmtpServer.Port = emailConnectivity.Port;
                SmtpServer.Credentials = new System.Net.NetworkCredential(emailConnectivity.Email, emailConnectivity.Password);
                SmtpServer.EnableSsl = true;

                foreach (string attachment in attachments)
                {
                    mail.Attachments.Add(new Attachment(attachment));
                }

                await SmtpServer.SendMailAsync(mail, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error sending email: {ex.Message}");
        }
    }
}

public readonly record struct EmailContent
{
    public EmailContent(string to, MissedPunchEmailData emailData)
    {
        this.Message = EmailContent.GetMissedPunchMessage(emailData.Username, emailData.DateTimes);
        this.Subject = $"Missed punch: {emailData.Username}";
        this.To = to;
        this.Hash = EmailContent.GetSha512(this);
    }
    public EmailContent(string to, PunchEntryEmailData emailData)
    {
        this.Message = EmailContent.GetPunchEntryRequestMessage(emailData.Username, emailData.NewDateTime, emailData.IsEditRequest, emailData.Note);
        this.Subject = $"New time entry request: {emailData.Username}";
        this.To = to;
        this.Hash = EmailContent.GetSha512(this);
    }

    public string To { get; init; }
    public string Subject { get; init; }
    public string Message { get; init; }
    public string Hash { get; init; }
    private static string GetSha512(EmailContent content)
    {
        byte[] data = [..Encoding.UTF8.GetBytes(content.To), ..Encoding.UTF8.GetBytes(content.Subject), .. Encoding.UTF8.GetBytes(content.Message)];
        byte[] result;
        result = SHA512.HashData(data);
        return Convert.ToHexString(result);
    }

    private static string GetPunchEntryRequestMessage(string username, DateTime newDateTime, bool isEditRequest, string? note)
    {
        StringBuilder message = new("This is an automated message from the TimeClock. ");

        message.AppendLine($"{username} is requesting a change in their time entries.");

        if (isEditRequest)
            message.AppendLine($"A new time entry is being requested for {newDateTime:g}");
        else
        {
            message.AppendLine($"A time entry is being requested to be changed to {newDateTime:g}");
        }

        if (!string.IsNullOrWhiteSpace(note))
        {
            message.AppendLine("The user had the following to say:");
            message.AppendLine(note);
            message.AppendLine();
        }

        message.AppendLine("Please login to TimeClock to approve or reject the request.");
        return message.ToString();
    }

    private static string GetMissedPunchMessage(string username, IEnumerable<DateOnly> dates)
    {
        StringBuilder message = new("This is an automated message from the TimeClock. ");

        message.AppendLine($"{username} has the following days with missing punches.");

        foreach (var date in dates)
        {
            message.AppendLine(date.ToLongDateString());
        }

        message.AppendLine("Please login to TimeClock to correct or ask employee to submit requests.");
        return message.ToString();
    }
}

public record struct PunchEntryEmailData(string Username, DateTime NewDateTime, bool IsEditRequest, string? Note);
public record struct MissedPunchEmailData(string Username, IEnumerable<DateOnly> DateTimes);
