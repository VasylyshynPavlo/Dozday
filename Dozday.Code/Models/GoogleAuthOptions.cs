namespace Dozday.Core.Models;

public class GoogleAuthOptions
{
    public string SecretKey { get; set; } = null!;
    public List<string> WhiteListedEmailDomains { get; set; } = [];
}