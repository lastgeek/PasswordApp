using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Application.Commands.CreateAccount;
using PasswordManager.Application.Commands.DeleteAccount;
using PasswordManager.Application.Commands.UpdateAccount;
using PasswordManager.Application.Queries.GetAccountDetails;
using PasswordManager.Application.Queries.GetAccounts;
using PasswordManager.Server.Services;
using PasswordManager.Shared;
using System.Security.Claims;

namespace PasswordManager.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly PasswordService _passwordService;
    private readonly IDataProtector _protector;

    public AccountController(IMediator mediator, PasswordService passwordService, IDataProtectionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(passwordService);
        ArgumentNullException.ThrowIfNull(provider);
        _mediator = mediator;
        _passwordService = passwordService;
        _protector = provider.CreateProtector("PasswordManager.AccountController");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDTO createAccountDTO, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return BadRequest("Invalid user ID.");
        }

        var command = new CreateAccountCommand()
        {
            UserId = userId,
            ServiceName = createAccountDTO.ServiceName,
            WebsiteUrl = createAccountDTO.WebsiteUrl,
            Username = createAccountDTO.Username,
            Password = _passwordService.EncryptPassword(createAccountDTO.Password, userIdString),
            Category = createAccountDTO.Category,
            PasswordStrength = createAccountDTO.PasswordStrength
        };

        await _mediator.Send(command);
        return Ok("Account was created.");
    }

    [Authorize]
    [HttpDelete("{AccountId}")]
    public async Task<IActionResult> DeleteAccount(string AccountId, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return BadRequest("Invalid user ID.");
        }

        if (!Guid.TryParse(_protector.Unprotect(AccountId), out var accountId))
        {
            return BadRequest("Invalid account ID.");
        }

        var command = new DeleteAccountCommand()
        {
            UserId = userId,
            AccountId = accountId
        };

        await _mediator.Send(command);

        return Ok("Account was deleted.");
    }

    [Authorize]
    [HttpGet("{AccountId}")]
    public async Task<ActionResult<AccountDTO>> GetAccountDetails(string AccountId, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return BadRequest("Invalid user ID.");
        }

        var id = Guid.Parse(_protector.Unprotect(AccountId));

        var query = new GetAccountDetailsQuery { UserId = userId, Id = id };
        var account = await _mediator.Send(query);

        if (account == null)
        {
            return NotFound("Account not found.");
        }

        var dto = new AccountDTO()
        {
            ServiceName = account.ServiceName,
            WebsiteUrl = account.WebsiteUrl,
            Username = account.Username,
            Password = _passwordService.DecryptPassword(account.Password.Password, userIdString),
            PasswordStrength = account.Password.PasswordStrength,
            Category = account.Category
        };

        return Ok(dto);
    }

    [Authorize]
    [HttpPut("{AccountId}")]
    public async Task<ActionResult> UpdateAccount(string AccountId, [FromBody] EditAccountDTO editAccountDTO, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return BadRequest("Invalid user ID.");
        }

        if (!Guid.TryParse(_protector.Unprotect(AccountId), out var accountId))
        {
            return BadRequest("Invalid account ID.");
        }

        var command = new UpdateAccountCommand()
        {
            UserId = userId,
            AccountId = accountId,
            ServiceName = editAccountDTO.ServiceName,
            WebsiteUrl = editAccountDTO.WebsiteUrl,
            Username = editAccountDTO.Username,
            Password = _passwordService.EncryptPassword(editAccountDTO.Password, userIdString),
            PasswordStrength = editAccountDTO.PasswordStrength,
            Category = editAccountDTO.Category
        };

        await _mediator.Send(command);

        return Ok("Account was update.");
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<AccountDTO>>> GetAccounts(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return BadRequest("Invalid user ID.");
        }

        var query = new GetAccountsQuery { UserId = userId };
        var accounts = await _mediator.Send(query);

        if (accounts == null)
        {
            return NotFound("Accounts not found.");
        }

        var dtos = new List<AccountDTO>();

        foreach (var account in accounts)
        {
            var dto = new AccountDTO
            {
                Id = _protector.Protect(account.Id.ToString()),
                ServiceName = account.ServiceName,
                Username = account.Username,
                LastUsed = account.LastUsed,
                WebsiteUrl = account.WebsiteUrl,
                Category = account.Category
            };
            dtos.Add(dto);
        }

        return Ok(dtos.OrderByDescending(a => a.LastUsed));
    }
}