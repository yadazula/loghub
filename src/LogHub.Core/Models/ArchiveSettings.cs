namespace LogHub.Core.Models
{
  public class ArchiveSettings
  {
    public string GlacierAWSAccessKey { get; set; }
    public string GlacierAWSSecretKey { get; set; }
    public string GlacierRegionName { get; set; }
    public string GlacierVault { get; set; }
    public string S3AWSAccessKey { get; set; }
    public string S3AWSSecretKey { get; set; }
    public string S3BucketName { get; set; }
    public string DiskPath { get; set; }
  }
}