using System.Linq;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Archiving
{
  public abstract class AbstractLogArchiver : ILogArchiver
  {
    protected AbstractLogArchiver(IDocumentSession documentSession)
    {
      Settings = documentSession.Query<Settings>().SingleOrDefault();
    }

    protected Settings Settings { get; set; }

    public abstract void Archive(Retention retention, string filePath);
  }
}