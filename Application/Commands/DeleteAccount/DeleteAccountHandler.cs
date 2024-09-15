using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Infrastracture.Persistance;

namespace PasswordManager.Application.Commands.DeleteAccount;

public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand>
{
    private readonly ApplicationDbContext _context;

    public DeleteAccountHandler(ApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.Where(a => a.UserId == request.UserId && a.Id == request.AccountId).Include(a => a.Password).FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("Account not found.");

        _context.AccountPasswords.Remove(account.Password);
        _context.Accounts.Remove(account);

        await _context.SaveChangesAsync(cancellationToken);
    }
}