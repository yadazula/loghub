using System.IO;
using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
  public class DiskArchiver : AbstractLogArchiver<DiskArchiveSetting>
  {
    protected override void Archive(DiskArchiveSetting setting, string filePath)
    {
      var fileName = Path.GetFileName(filePath);
      var destFileName = Path.Combine(setting.Path, fileName);
      File.Copy(filePath, destFileName);
    }
  }
}