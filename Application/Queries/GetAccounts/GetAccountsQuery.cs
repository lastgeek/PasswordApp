using Domain.Models;
using MediatR;

namespace PasswordManager.Application.Queries.GetAccounts;

public class GetAccountsQuery : IRequest<List<Account>>
{
    public Guid UserId { get; set; }
}