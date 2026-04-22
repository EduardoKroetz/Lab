using Lab.Application.Common.Interfaces;
using Lab.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Lab.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>, IUser, ITenantEntity
{
    public Guid TenantId { get; set; }
}
