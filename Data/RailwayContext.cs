using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Models;
namespace RailwayManagementSystemAPI.Data
{
    public class RailwayContext : DbContext
    {
        public RailwayContext(DbContextOptions<RailwayContext> options) : base(options)
        {
        }
        public DbSet<Models.Train> Trains { get; set; } = null!;
        public DbSet<Models.Station> Stations { get; set; } = null!;

        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Models.RouteStation> RouteStations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RouteStation>()
                .HasOne(rs => rs.Route)
                .WithMany(r => r.RouteStations)
                .HasForeignKey(rs => rs.RouteId);

            modelBuilder.Entity<RouteStation>()
                .HasOne(rs => rs.Station)
                .WithMany()
                .HasForeignKey(rs => rs.StationId);

            modelBuilder.Entity<RouteStation>()
                .HasIndex(rs => new { rs.RouteId, rs.Order })
                .IsUnique();
        }
    }
}
