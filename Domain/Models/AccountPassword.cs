using Domain.Common;

namespace Domain.Models;

public class AccountPassword : BaseEntity
{
    public required int PasswordStrength { get; set; }
    public required string Password { get; set; }
}