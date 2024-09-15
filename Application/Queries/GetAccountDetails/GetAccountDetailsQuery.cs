using Domain.Models;
using MediatR;

namespace PasswordManager.Application.Queries.GetAccountDetails;

public class GetAccountDetailsQuery : IRequest<Account>
{
    public Guid UserId { get; set; }
    public Guid Id { get; set; }
}