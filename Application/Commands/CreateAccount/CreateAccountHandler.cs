using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Infrastracture.Persistance;

namespace PasswordManager.Application.Commands.CreateAccount;

public class CreateAccountHandler : IRequestHandler<CreateAccountCommand>
{
    private readonly ApplicationDbContext _context;

    public CreateAccountHandler(ApplicationDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account()
        {
            UserId = request.UserId,
            ServiceName = request.ServiceName,
            WebsiteUrl = request.WebsiteUrl,
            Username = request.Username,
            Password = new AccountPassword() 
            { 
                Password = request.Password, 
                PasswordStrength = request.PasswordStrength 
            },
            Category = request.Category
        };

        await _context.Accounts.AddAsync(account, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}