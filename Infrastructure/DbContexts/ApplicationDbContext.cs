using Application.Contracts.Services.AuthServices;
using Application.Statics.Configurations;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.DbContexts
{
    public partial class ApplicationDbContext : DbContext
    {
        private readonly IAuthenticatedUserService _autheticatedUserService;
        private readonly string UserName;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options
        , IAuthenticatedUserService autheticatedUserService
        )
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            if (base.Database.IsRelational())
            {
                base.Database.SetCommandTimeout(TimeSpan.FromMinutes(DbSettings.TimeoutInMinutes));
            }
            _autheticatedUserService = autheticatedUserService;
            UserName = _autheticatedUserService.GetUsernameFromClaims();

        }
    }
}
