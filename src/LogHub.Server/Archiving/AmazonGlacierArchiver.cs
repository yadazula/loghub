using System.IO;
using Amazon.Glacier.Transfer;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Server.Archiving
{
  public class AmazonGlacierArchiver : AbstractLogArchiver
  {
    public AmazonGlacierArchiver(IDocumentSession documentSession)
      : base(documentSession)
    {
    }

    public override void Archive(Retention retention, string filePath)
    {
      if (!retention.ArchiveToGlacier)
        return;

      using (var transferManager = new ArchiveTransferManager(Settings.Archive.GlacierAccessKey, Settings.Archive.GlacierSecretKey, Amazon.RegionEndpoint.GetBySystemName(Settings.Archive.GlacierRegionName)))
      {
        transferManager.Upload(Settings.Archive.GlacierVault, Path.GetFileNameWithoutExtension(filePath), filePath);
      }
    }
  }
}