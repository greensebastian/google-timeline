using Model.Timeline.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<TimelineData> TimelineData { get; set; }
        public DbSet<DbActivitySegment> ActivitySegments { get; set; }
        public DbSet<DbPlaceVisit> PlaceVisits { get; set; }
        public DbSet<DbLocationVisit> LocationVisits { get; set; }
        public DbSet<DbLocation> Locations { get; set; }
        public DbSet<DbWaypoint> Waypoints { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
