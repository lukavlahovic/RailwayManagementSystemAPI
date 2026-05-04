using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RailwayManagementSystemAPI.Data;
using RailwayManagementSystemAPI.Mapping;

namespace RailwayManagementSystemAPI.Tests
{
    public abstract class TestBase : IDisposable
    {
        protected readonly RailwayContext Context;
        protected readonly IMapper Mapper;
        private readonly SqliteConnection _connection;

        protected TestBase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<RailwayContext>()
                .UseSqlite(_connection)
                .Options;

            Context = new RailwayContext(options);
            Context.Database.EnsureCreated();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            Mapper = config.CreateMapper();
        }
        public void Dispose()
        {
            Context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }
    }
}
