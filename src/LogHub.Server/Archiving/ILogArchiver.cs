using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
  public interface ILogArchiver
  {
    void Archive(Retention retention, string filePath);
  }
}