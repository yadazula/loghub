using System.IO;
using Amazon.Glacier.Transfer;
using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
  public class AmazonGlacierArchiver : AbstractLogArchiver<AmazonGlacierArchiveSetting>
  {
    protected override void Archive(AmazonGlacierArchiveSetting setting, string filePath)
    {
      using (var transferManager = new ArchiveTransferManager(setting.AWSAccessKey, setting.AWSSecretKey, Amazon.RegionEndpoint.GetBySystemName(setting.RegionName)))
      {
        transferManager.Upload(setting.Vault, Path.GetFileNameWithoutExtension(filePath), filePath);
      }
    }
  }
}