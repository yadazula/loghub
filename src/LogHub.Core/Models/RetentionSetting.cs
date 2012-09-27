using System;
using System.Collections.Generic;

namespace LogHub.Core.Models
{
  public class RetentionSetting
  {
    public string Id { get; set; }
    public string Source { get; set; }
    public ushort Days { get; set; }
    public bool ArchiveToGlacier { get; set; }
    public bool ArchiveToS3 { get; set; }
    public bool ArchiveToDisk { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public bool NeedsArchiving
    {
      get
      {
        return (ArchiveToS3 || ArchiveToGlacier || ArchiveToDisk);
      }
    }
  }
}