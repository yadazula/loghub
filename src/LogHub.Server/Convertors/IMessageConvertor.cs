using LogHub.Core.Models;

namespace LogHub.Server.Convertors
{
  public interface ILogMessageConvertor
  {
    LogMessage Convert(RawMessage rawMessage);
  }
}