using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using LogHub.Core.Extensions;
using LogHub.Core.Indexes;
using LogHub.Core.Models;
using Raven.Client;
using Raven.Client.Linq;

namespace LogHub.Server.Tasks
{
	public class NotificationScheduledTask : IScheduledTask
	{
		private readonly IDocumentStore documentStore;

		public NotificationScheduledTask(IDocumentStore documentStore)
		{
			this.documentStore = documentStore;
		}

		public TimeSpan Period
		{
			get { return TimeSpan.FromMinutes(5); }
		}

		public void Execute()
		{
			using (var documentSession = documentStore.OpenSession())
			{
				var logAlerts = documentSession.Query<LogAlert>().ToList();
				foreach (var logAlert in logAlerts)
				{
					var statistics = QueryStatistics(documentSession, logAlert);

					if (statistics.TotalResults >= logAlert.MessageCount)
					{
						SendAlertMail(documentSession, logAlert, statistics.TotalResults);
					}
				}
			}
		}

		private RavenQueryStatistics QueryStatistics(IDocumentSession documentSession, LogAlert logAlert)
		{
			var cutoffDate = DateTime.UtcNow.AddMinutes(-logAlert.Minutes.TotalMinutes);

			RavenQueryStatistics stats;
			IQueryable<LogMessage> query = documentSession.Query<LogMessage, LogMessage_Search>()
				.Statistics(out stats)
				.Where(x => x.Date >= cutoffDate);

			if (logAlert.Host.IsNotNullOrWhiteSpace())
			{
				query = query.Where(x => x.Host.StartsWith(logAlert.Host));
			}

			if (logAlert.Source.IsNotNullOrWhiteSpace())
			{
				query = query.Where(x => x.Source.StartsWith(logAlert.Source));
			}

			if (logAlert.Message.IsNotNullOrWhiteSpace())
			{
				query = query.Where(x => x.Message.StartsWith(logAlert.Message));
			}

			if (logAlert.Level != LogLevel.None)
			{
				query = query.Where(x => x.Level >= logAlert.Level);
			}

			query.Take(0).ToList();
			return stats;
		}

		private void SendAlertMail(IDocumentSession documentSession, LogAlert logAlert, int messageCount)
		{
			var settings = documentSession.Query<Settings>().Single().Notification;

			var mail = new MailMessage {From = new MailAddress(settings.FromAddress)};

			if (logAlert.EmailToList.Count == 0)
			{
				var user = documentSession.Load<User>(logAlert.User);
				mail.To.Add(user.Email);
			}
			else
			{
				foreach (var emailTo in logAlert.EmailToList)
				{
					mail.To.Add(emailTo);	
				}
			}

			mail.Subject = string.Format("[loghub] Alert for {0}", logAlert.Name);
			mail.BodyEncoding = Encoding.UTF8;
			mail.Body =
				string.Format("Message limit is exceeded for alert named '{0}'. Received {1} messages between {2} and {3}.",
				              logAlert.Name,
				              messageCount,
				              DateTime.UtcNow.AddMinutes(-logAlert.Minutes.TotalMinutes).ToString("yyyy-MM-dd HH:mm:ss.fff K"),
				              DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff K")
					);

			var smtpClient = new SmtpClient(settings.SmtpServer, settings.SmtpPort);
			smtpClient.Credentials = new System.Net.NetworkCredential(settings.SmtpUsername, settings.SmtpPassword);
			smtpClient.EnableSsl = settings.SmtpEnableSsl;
			smtpClient.Send(mail);
		}
	}
}