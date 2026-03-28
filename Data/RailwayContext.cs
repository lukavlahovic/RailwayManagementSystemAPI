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
        public DbSet<Models.TrainType> TrainTypes { get; set; } = null!;
        public DbSet<Models.Station> Stations { get; set; } = null!;
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Models.RouteStation> RouteStations { get; set; }
        public DbSet<Models.Trip> Trip { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Train>()
                .HasOne(t => t.TrainType)
                .WithMany()
                .HasForeignKey(t => t.TrainTypeId);

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

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Train)
                .WithMany()
                .HasForeignKey(t => t.TrainId);

            modelBuilder.Entity<Trip>()
                .HasOne(r => r.Route)
                .WithMany()
                .HasForeignKey(r => r.RouteId);
        }
    }
}
