using MediatR;

namespace PasswordManager.Application.Commands.DeleteAccount;

public class DeleteAccountCommand : IRequest
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
}