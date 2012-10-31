using System.Linq;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Core.Extensions
{
	public static class DocumentSessionExtensions
	{
		public static User GetUserByUsername(this IDocumentSession documentSession, string username)
		{
			var user = documentSession.Query<User>().SingleOrDefault(x => x.Username == username);
			return user;
		}

		public static Settings GetSettings(this IDocumentSession documentSession)
		{
			var settings = documentSession.Load<Settings>(Settings.DocId);
			return settings;
		}

		public static ThroughputInfo GetThroughputInfo(this IDocumentSession documentSession)
		{
			var throughputInfo = documentSession.Load<ThroughputInfo>(ThroughputInfo.DocId);
			return throughputInfo;
		}
	}
}