using LogHub.Core.Models;

namespace LogHub.Server.Retention
{
  public class DiskArchiver : ILogArchiver
  {
    private readonly DiskArchiveSetting setting;

    public DiskArchiver(DiskArchiveSetting setting)
    {
      this.setting = setting;
    }

    public void Archive(string file)
    {
    }
  }
}