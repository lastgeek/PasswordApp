using Domain.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PasswordManager.Domain.Models;

namespace PasswordManager.Infrastracture.Persistance;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<AccountPassword> AccountPasswords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().HasKey(e => e.Id);
        modelBuilder.Entity<AccountPassword>().HasKey(e => e.Id);

        base.OnModelCreating(modelBuilder);
    }
}