using MediatR;

namespace PasswordManager.Application.Commands.CreateAccount;

public class CreateAccountCommand : IRequest
{
    public Guid UserId { get; set; }
    public required string ServiceName { get; set; }
    public required string WebsiteUrl { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required int PasswordStrength { get; set; }
    public required string Category { get; set; }
}