namespace LogHub.Server.Retention
{
  public interface ILogArchiver
  {
    void Archive(string file);
  }
}