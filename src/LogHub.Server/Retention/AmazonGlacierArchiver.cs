using LogHub.Core.Models;

namespace LogHub.Server.Retention
{
  public class AmazonGlacierArchiver : ILogArchiver
  {
    private readonly AmazonGlacierArchiveSetting setting;

    public AmazonGlacierArchiver(AmazonGlacierArchiveSetting setting)
    {
      this.setting = setting;
    }

    public void Archive(string file)
    {
    }
  }
}