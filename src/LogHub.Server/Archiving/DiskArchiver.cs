using System.IO;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Archiving
{
  public class DiskArchiver : AbstractLogArchiver
  {
    public DiskArchiver(IDocumentStore documentStore)
      : base(documentStore)
    {
    }

    public override void Archive(Retention retention, string filePath)
    {
      if (!retention.ArchiveToDisk)
        return;

      var settings = GetSettings();
      var fileName = Path.GetFileName(filePath);
      var destFileName = Path.Combine(settings.Archive.DiskPath, fileName);
      File.Copy(filePath, destFileName);
    }
  }
}