using FacebookClone.Core.IRepository;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;


namespace FacebookClone.EF.Repository
{

	public class EmailService : IEmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string body)
		{
			var emailSettings = _configuration.GetSection("EmailSettings");

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("Facebook Clone", emailSettings["SenderEmail"]));
			message.To.Add(new MailboxAddress("", toEmail));
			message.Subject = subject;

			var bodyBuilder = new BodyBuilder
			{
				HtmlBody = body
			};
			message.Body = bodyBuilder.ToMessageBody();

			using var client = new SmtpClient();
			try
			{
				await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]), SecureSocketOptions.StartTls);
				await client.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["SenderPassword"]);
				await client.SendAsync(message);
				await client.DisconnectAsync(true);
			}
			catch (Exception ex)
			{
				throw new Exception($"Email could not be sent: {ex.Message}");
			}
		}
	}

}
