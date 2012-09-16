using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using LogHub.Web.Infrastructure.AutoMapper;
using LogHub.Web.Infrastructure.Common;
using LogHub.Web.Infrastructure.Indexes;
using LogHub.Web.Models;
using LogHub.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace LogHub.Web.Controllers
{
  public class SearchController : AbstractApiController
  {
    public SearchController(IDocumentSession documentSession)
      : base(documentSession)
    {
    }

    public PagedResult<LogMessageView> Get([FromUri]SearchLogFilter searchLogFilter)
    {
      RavenQueryStatistics stats;
      var logMessages = DocumentSession.Query<LogMessage, LogMessage_Search>()
                                 .Statistics(out stats)
                                 .FilterBy(searchLogFilter)
                                 .OrderByDescending(x => x.Date)
                                 .Paging(searchLogFilter.Page, searchLogFilter.MessageCount)
                                 .ToList()
                                 .MapTo<LogMessageView>();

      var result = new PagedResult<LogMessageView>
      {
        Page = searchLogFilter.Page ?? 1,
        Total = stats.TotalResults,
        Models = logMessages
      };

      return result;
    }
  }
}