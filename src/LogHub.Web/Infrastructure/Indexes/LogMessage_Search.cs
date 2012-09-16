using System.Linq;
using LogHub.Web.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace LogHub.Web.Infrastructure.Indexes
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

      Indexes.Add(x => x.Host, FieldIndexing.Analyzed);
      Indexes.Add(x => x.Source, FieldIndexing.Analyzed);
      Indexes.Add(x => x.Message, FieldIndexing.Analyzed);
      Indexes.Add(x => x.Logger, FieldIndexing.Analyzed);
    }
  }
}