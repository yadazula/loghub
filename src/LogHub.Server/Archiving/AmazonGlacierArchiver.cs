using System.IO;
using Amazon.Glacier.Transfer;
using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
  public class AmazonGlacierArchiver : ILogArchiver
  {
    public void Archive(RetentionSetting retentionSetting, ArchiveSettings setting, string filePath)
    {
      if (!retentionSetting.ArchiveToGlacier)
        return;

      using (var transferManager = new ArchiveTransferManager(setting.GlacierAWSAccessKey, setting.GlacierAWSSecretKey, Amazon.RegionEndpoint.GetBySystemName(setting.GlacierRegionName)))
      {
        transferManager.Upload(setting.GlacierVault, Path.GetFileNameWithoutExtension(filePath), filePath);
      }
    }
  }
}