using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Infrastracture.Persistance;

namespace PasswordManager.Application.Queries.GetAccountDetails;

public class GetAccountDetailsHandler : IRequestHandler<GetAccountDetailsQuery, Account>
{
    private readonly ApplicationDbContext _context;

    public GetAccountDetailsHandler(ApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<Account> Handle(GetAccountDetailsQuery request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.Where(a => a.UserId == request.UserId)
                                             .Include(a => a.Password)
                                             .AsNoTracking()
                                             .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
                                             ?? throw new InvalidOperationException("Account not found.");

        return account;
    }
}