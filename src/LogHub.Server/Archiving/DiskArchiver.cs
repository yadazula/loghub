using System.IO;
using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
  public class DiskArchiver : ILogArchiver
  {
    public void Archive(RetentionSetting retentionSetting, ArchiveSettings setting, string filePath)
    {
      if (!retentionSetting.ArchiveToDisk)
        return;

      var fileName = Path.GetFileName(filePath);
      var destFileName = Path.Combine(setting.DiskPath, fileName);
      File.Copy(filePath, destFileName);
    }
  }
}