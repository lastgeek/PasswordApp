using Domain.Common;

namespace Domain.Models;

public class Account : BaseEntity
{
    public Guid UserId { get; set; }
    public required string ServiceName { get; set; }
    public required string WebsiteUrl { get; set; }
    public required string Username { get; set; }
    public required AccountPassword Password { get; set; }
    public required string Category { get; set; }
    public DateTime LastUsed { get; set; } = DateTime.Now;
}