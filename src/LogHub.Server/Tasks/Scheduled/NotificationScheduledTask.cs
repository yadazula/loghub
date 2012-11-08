using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using LogHub.Core.Extensions;
using LogHub.Core.Indexes;
using LogHub.Core.Models;
using Raven.Client;
using Raven.Client.Linq;

namespace LogHub.Server.Tasks.Scheduled
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
					RavenQueryStatistics stats;
					var messages = QueryMessages(documentSession, logAlert, out stats);
					var isOverLimit = stats.TotalResults >= logAlert.MessageCount;
					var isNotifiedBefore = logAlert.LastNotificationDate.AddMinutes(logAlert.Minutes.TotalMinutes) >= DateTime.UtcNow;

					if (isOverLimit && isNotifiedBefore == false)
					{
						SendAlertMail(documentSession, logAlert, messages, stats);
						logAlert.LastNotificationDate = DateTime.UtcNow;
					}
				}

				documentSession.SaveChanges();
			}
		}

		private IList<LogMessage> QueryMessages(IDocumentSession documentSession, LogAlert logAlert, out RavenQueryStatistics stats)
		{
			var cutoffDate = DateTime.UtcNow.AddMinutes(-logAlert.Minutes.TotalMinutes);

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

			var logMessages = query.Take(10).ToList();
			return logMessages;
		}

		private void SendAlertMail(IDocumentSession documentSession, LogAlert logAlert, IList<LogMessage> messages, RavenQueryStatistics stats)
		{
			var notificationSettings = documentSession.GetSettings().Notification;

			if (IsValid(notificationSettings) == false)
			{
				return;
			}

			var mail = new MailMessage { From = new MailAddress(notificationSettings.FromAddress) };

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

			var messageText = new StringBuilder(); 
			foreach (var message in messages)
			{
				messageText.AppendFormat("{0} {1} {2} {3} {4} {5}\n",
																	message.Date.ToOffsetString(),
																	message.Host,
																	message.Source,
																	message.Logger,
																	message.Level,
																	message.Message);
			}

			mail.Subject = string.Format("[loghub] Alert for {0}", logAlert.Name);
			mail.BodyEncoding = Encoding.UTF8;
			mail.Body = string.Format("Message limit is exceeded for alert named '{0}'. Received {1} messages between {2} and {3}.\n\n{4}",
																logAlert.Name,
																stats.TotalResults,
																DateTime.UtcNow.AddMinutes(-logAlert.Minutes.TotalMinutes).ToOffsetString(),
																DateTime.UtcNow.ToOffsetString(),
																messageText);

			var smtpClient = new SmtpClient(notificationSettings.SmtpServer, notificationSettings.SmtpPort);
			smtpClient.Credentials = new System.Net.NetworkCredential(notificationSettings.SmtpUsername, notificationSettings.SmtpPassword);
			smtpClient.EnableSsl = notificationSettings.SmtpEnableSsl;
			smtpClient.Send(mail);
		}

		private bool IsValid(Settings.NotificationSettings notificationSettings)
		{
			if (notificationSettings.SmtpServer.IsNullOrWhiteSpace())
				return false;

			if (notificationSettings.SmtpPort <= 0)
				return false;

			if (notificationSettings.SmtpUsername.IsNullOrWhiteSpace())
				return false;

			if (notificationSettings.SmtpPassword.IsNullOrWhiteSpace())
				return false;

			if (notificationSettings.FromAddress.IsNullOrWhiteSpace())
				return false;

			return true;
		}
	}
}