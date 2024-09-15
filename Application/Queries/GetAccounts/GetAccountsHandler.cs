using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Infrastracture.Persistance;

namespace PasswordManager.Application.Queries.GetAccounts;

public class GetAccountsHandler : IRequestHandler<GetAccountsQuery, List<Account>>
{
    private readonly ApplicationDbContext _context;

    public GetAccountsHandler(ApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<List<Account>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _context.Accounts.Where(a => a.UserId == request.UserId)
                                              .AsNoTracking()
                                              .ToListAsync(cancellationToken);

        return accounts;
    }
}