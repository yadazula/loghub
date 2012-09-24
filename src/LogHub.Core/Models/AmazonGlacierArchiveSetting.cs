namespace LogHub.Core.Models
{
  public class AmazonGlacierArchiveSetting : IArchiveSetting
  {
    public string AWSAccessKey { get; set; }
    public string AWSSecretKey { get; set; }
    public string Vault { get; set; }
  }
}