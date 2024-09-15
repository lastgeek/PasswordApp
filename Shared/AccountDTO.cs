namespace PasswordManager.Shared;

public class AccountDTO
{
    public string Id { get; set; }
    public string ServiceName { get; set; }
    public string WebsiteUrl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int PasswordStrength {  get; set; }
    public string Category { get; set; }
    public DateTime LastUsed { get; set; }
}