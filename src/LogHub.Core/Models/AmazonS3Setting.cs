namespace LogHub.Core.Models
{
  public class AmazonS3Setting : IArchiveSetting
  {
    public string AWSAccessKey { get; set; }
    public string AWSSecretKey { get; set; }
    public string BucketName { get; set; }
  }
}