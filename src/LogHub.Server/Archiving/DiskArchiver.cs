using System.IO;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Archiving
{
  public class DiskArchiver : AbstractLogArchiver
  {
    public DiskArchiver(IDocumentSession documentSession)
      : base(documentSession)
    {
    }

    public override void Archive(Retention retention, string filePath)
    {
      if (!retention.ArchiveToDisk)
        return;

      var fileName = Path.GetFileName(filePath);
      var destFileName = Path.Combine(Settings.Archive.DiskPath, fileName);
      File.Copy(filePath, destFileName);
    }
  }
}