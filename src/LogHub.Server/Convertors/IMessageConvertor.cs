using LogHub.Server.Core;

namespace LogHub.Server.Convertors
{
  public interface ILogMessageConvertor
  {
    LogMessage Convert(byte[] payload);
  }
}