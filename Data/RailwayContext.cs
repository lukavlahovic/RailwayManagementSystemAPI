using Microsoft.EntityFrameworkCore;
namespace RailwayManagementSystemAPI.Data
{
    public class RailwayContext : DbContext
    {
        public RailwayContext(DbContextOptions<RailwayContext> options) : base(options)
        {
        }
        public DbSet<Models.Train> Trains { get; set; } = null!;
        public DbSet<Models.Station> Stations { get; set; } = null!;
    }
}
