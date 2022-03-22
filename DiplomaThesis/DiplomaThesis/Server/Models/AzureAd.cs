namespace DiplomaThesis.Server.Models;

public class AzureAd
{
    public string AuthorityUri { get; set; }
    public string DirectoryId { get; set; }
    public string GrantType { get; set; }
    public string Resource { get; set; }
    public string ClientId { get; set; }
}