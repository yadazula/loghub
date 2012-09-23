using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LogHub.Core.Models;
using Raven.Client;
using Raven.Client.Linq;

namespace LogHub.Web.Controllers
{
  public class RetentionController : AbstractApiController
  {
    public RetentionController(IDocumentSession documentSession)
      : base(documentSession)
    {
    }

    public IEnumerable<LogRetention> Get()
    {
      var items = DocumentSession.Query<LogRetention>()
                                 .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                                 .Include(x => x.CreatedBy)
                                 .ToList();

      foreach (var item in items)
      {
        DocumentSession.Advanced.Evict(item);
        item.CreatedBy = DocumentSession.Load<User>(item.CreatedBy).Name;
      }

      return items;
    }

    public void Post(LogRetention logRetention)
    {
      Save(logRetention);
    }

    public void Put(LogRetention logRetention)
    {
      Save(logRetention);
    }

    private void Save(LogRetention logRetention)
    {
      var user = DocumentSession.Query<User>().Single(x => x.Username == User.Identity.Name);
      logRetention.CreatedBy = DocumentSession.Advanced.GetDocumentId(user);
      logRetention.CreatedAt = DateTimeOffset.Now;
      DocumentSession.Store(logRetention);
    }

    public void Delete(string id)
    {
      var item = DocumentSession.Load<LogRetention>(id);
      DocumentSession.Delete(item);
    }
  }
}