using System;
using System.Collections.Generic;

namespace LogHub.Core.Models
{
  public class RetentionSetting
  {
    public string Id { get; set; }
    public string Source { get; set; }
    public ushort Days { get; set; }
    public IList<IArchiveSetting> ArchiveSettings { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public RetentionSetting()
    {
      ArchiveSettings = new List<IArchiveSetting>();
    }
  }
}