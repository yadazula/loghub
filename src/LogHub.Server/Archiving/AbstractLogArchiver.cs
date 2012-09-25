using System;
using LogHub.Core.Models;

namespace LogHub.Server.Archiving
{
  public abstract class AbstractLogArchiver<TArchiveSetting> : ILogArchiver where TArchiveSetting : IArchiveSetting
  {
    public void Archive(IArchiveSetting setting, string filePath)
    {
      Archive((TArchiveSetting)setting, filePath);
    }

    protected abstract void Archive(TArchiveSetting setting, string filePath);
  }
}