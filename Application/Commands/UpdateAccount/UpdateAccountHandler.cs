using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Infrastracture.Persistance;

namespace PasswordManager.Application.Commands.UpdateAccount;

public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand>
{
    private readonly ApplicationDbContext _context;

    public UpdateAccountHandler(ApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.Include(a => a.Password).Where(a => a.UserId == request.UserId && a.Id == request.AccountId).FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("Account not found.");

        account.ServiceName = request.ServiceName;
        account.WebsiteUrl = request.WebsiteUrl;
        account.Username = request.Username;
        if (account.Password.Password != request.Password)
        {
            account.LastUsed = DateTime.Now;
        }
        account.Password.Password = request.Password;
        account.Password.PasswordStrength = request.PasswordStrength;
        account.Category = request.Category;

        await _context.SaveChangesAsync(cancellationToken);
    }
}