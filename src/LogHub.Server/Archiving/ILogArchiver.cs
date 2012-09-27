using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
  public interface ILogArchiver
  {
    void Archive(RetentionSetting retentionSetting, ArchiveSettings setting, string filePath);
  }
}