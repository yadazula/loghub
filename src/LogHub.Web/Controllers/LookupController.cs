using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using LogHub.Web.Infrastructure.Common;
using LogHub.Web.Models;
using LogHub.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace LogHub.Web.Controllers
{
  public class LookupController : AbstractApiController
  {
    public LookupController(IDocumentSession documentSession)
      : base(documentSession)
    {
    }

    public IEnumerable<string> Get([FromUri]LookupFilter lookupFilter)
    {
      if (lookupFilter.Type == "source")
      {
        return GetSources(lookupFilter.Query);
      }

      if (lookupFilter.Type == "host")
      {
        return GetHosts(lookupFilter.Query);
      }

      return Enumerable.Empty<string>();
    }

    private IEnumerable<string> GetSources(string query)
    {
      if (query.IsNullOrWhiteSpace())
      {
        return DocumentSession.Query<LogMessage>()
                              .Select(x => x.Source)
                              .Distinct()
                              .ToList()
                              .OrderBy(x => x);
      }

      return DocumentSession.Query<LogMessage>()
                            .Where(x => x.Source.StartsWith(query))
                            .Select(x => x.Source)
                            .Distinct()
                            .ToList()
                            .OrderBy(x => x);
    }

    private IEnumerable<string> GetHosts(string query)
    {
      if (query.IsNullOrWhiteSpace())
      {
        return DocumentSession.Query<LogMessage>()
                              .Select(x => x.Host)
                              .Distinct()
                              .ToList()
                              .OrderBy(x => x);
      }

      return DocumentSession.Query<LogMessage>()
                            .Where(x => x.Host.StartsWith(query))
                            .Select(x => x.Host)
                            .Distinct()
                            .ToList()
                            .OrderBy(x => x);
    }
  }
}