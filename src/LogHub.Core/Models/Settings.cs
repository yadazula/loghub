using System;

namespace LogHub.Core.Models
{
  public class Settings
  {
    public ArchiveSettings Archive { get; set; }
    public NotificationSettings Notification { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Settings()
    {
      Archive = new ArchiveSettings();
      Notification = new NotificationSettings();
    }

    public class NotificationSettings
    {
      public string SmtpServer { get; set; }
      public int SmtpPort { get; set; }
      public string SmtpUsername { get; set; }
      public string SmtpPassword { get; set; }
      public bool SmtpEnableSsl { get; set; }
      public string FromAddress { get; set; }
    }

    public class ArchiveSettings
    {
      public string GlacierAccessKey { get; set; }
      public string GlacierSecretKey { get; set; }
      public string GlacierRegionName { get; set; }
      public string GlacierVault { get; set; }

      public string S3AccessKey { get; set; }
      public string S3SecretKey { get; set; }
      public string S3BucketName { get; set; }

      public string DiskPath { get; set; }
    }
  }
}