using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Repository
{
    public interface IUserClaimsRepository : IDataRepository<UserClaim>
    {
        IQueryable<UserClaim> GetClaimsForUser(Guid userId, ISorting<UserClaim>? sorting = null, bool? tracking = null);
        Task<IEnumerable<UserClaim>> GetClaimsForUserAsync(Guid userId, ISorting<UserClaim>? sorting = null, bool? tracking = null, CancellationToken cancellationToken = default, params Expression<Func<UserClaim, object>>[] includes);
        bool UserHasClaim(Guid userId, Guid authClaimId, bool? tracking = null);
        Task<bool> UserHasClaimAsync(Guid userId, Guid authClaimId, bool? tracking = null);
        bool UserHasClaim(Guid userId, string authClaimType, bool? tracking = null);
        Task<bool> UserHasClaimAsync(Guid userId, string authClaimType, bool? tracking = null);
    }

    public class UserClaimsRepository : DataRepository<UserClaim>, IUserClaimsRepository
    {
        public UserClaimsRepository(TimeClockContext context) : base(context) { }

        public IQueryable<UserClaim> GetClaimsForUser(Guid userId, ISorting<UserClaim>? sorting = null, bool? tracking = null)
        {
            return base.SortedDbSet(sorting, tracking).Where(x => x.UserId == userId);
        }

        public async Task<IEnumerable<UserClaim>> GetClaimsForUserAsync(Guid userId, ISorting<UserClaim>? sorting = null, bool? tracking = null, CancellationToken cancellationToken = default, params Expression<Func<UserClaim, object>>[] includes)
        {
            return await base.SortedDbSet(sorting, tracking).IncludeMultiple(includes).Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        }

        public bool UserHasClaim(Guid userId, Guid authClaimId, bool? tracking = null)
        {
            return base.GetTracker(tracking).Any(x => x.UserId == userId && x.AuthorizationClaimId == authClaimId);
        }

        public Task<bool> UserHasClaimAsync(Guid userId, Guid authClaimId, bool? tracking = null)
        {
            return base.GetTracker(tracking).AnyAsync(x => x.UserId == userId && x.AuthorizationClaimId == authClaimId);
        }

        public bool UserHasClaim(Guid userId, string authClaimType, bool? tracking = null)
        {
            return base.GetTracker(tracking).Any(x => x.UserId == userId && x.AuthorizationClaim.Type == authClaimType);
        }

        public Task<bool> UserHasClaimAsync(Guid userId, string authClaimType, bool? tracking = null)
        {
            return base.GetTracker(tracking).AnyAsync(x => x.UserId == userId && x.AuthorizationClaim.Type == authClaimType);
        }
    }
}
