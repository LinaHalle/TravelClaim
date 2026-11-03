using Microsoft.EntityFrameworkCore;
using TravelClaimModel = TravelClaim.Web.Models.TravelClaim;
using TravelClaim.Web.Models;

namespace TravelClaim.Web.Data;

public class TravelClaimContext : DbContext
{
    public TravelClaimContext(DbContextOptions<TravelClaimContext> options)
        : base(options)
    {
    }

    public DbSet<TravelClaimModel> TravelClaims {get; set;}
    public DbSet<DailyCalculation> DailyCalculations {get; set;}
}