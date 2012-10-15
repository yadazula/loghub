using System.Linq;
using LogHub.Core.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace LogHub.Core.Indexes
{
	public class LogMessage_Search : AbstractIndexCreationTask<LogMessage>
	{
		public LogMessage_Search()
		{
			Map = logs => from log in logs
										select new
											{
												log.Host,
												log.Source,
												log.Message,
												log.Logger,
												log.Level,
												log.Date
											};

			TransformResults = (database, logs) => from log in logs
																						 let message = (log.Message.Length > 250) ? log.Message.Substring(0, 250) : log.Message
																						 select new
																							 {
																								 log.Id,
																								 log.Host,
																								 log.Source,
																								 log.Logger,
																								 log.Level,
																								 Message = message,
																								 log.Date,
																								 log.Properties
																							 };

			Indexes.Add(x => x.Host, FieldIndexing.Analyzed);
			Indexes.Add(x => x.Source, FieldIndexing.Analyzed);
			Indexes.Add(x => x.Message, FieldIndexing.Analyzed);
			Indexes.Add(x => x.Logger, FieldIndexing.Analyzed);
		}
	}
}