using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using LogHub.Core.Indexes;
using LogHub.Core.Models;
using LogHub.Web.Infrastructure.AutoMapper;
using LogHub.Web.Infrastructure.Common;
using LogHub.Web.ViewModels;
using Raven.Client;

namespace LogHub.Web.Controllers
{
  public class RecentController : AbstractApiController
  {
    public RecentController(IDocumentSession documentSession)
      : base(documentSession)
    {
    }

    public IEnumerable<LogMessageView> Get([FromUri]RecentLogFilter recentLogFilter)
    {
      var logMessages = DocumentSession.Query<LogMessage, LogMessage_Search>()
                                 .FilterBy(recentLogFilter)
                                 .OrderByDescending(x => x.Date)
                                 .Paging(pageSize: recentLogFilter.MessageCount)
                                 .ToList()
                                 .MapTo<LogMessageView>();

      return logMessages;
    }
  }
}