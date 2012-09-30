using System.Linq;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Archiving
{
  public abstract class AbstractLogArchiver : ILogArchiver
  {
    private readonly IDocumentStore documentStore;

    protected AbstractLogArchiver(IDocumentStore documentStore)
    {
      this.documentStore = documentStore;
    }

    protected Settings GetSettings()
    {
      using (var documentSession = documentStore.OpenSession())
      {
        return documentSession.Query<Settings>().Single();
      }
    }

    public abstract void Archive(Retention retention, string filePath);
  }
}