using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL
{
    public class AppDatabaseContext : DbContext
    {
        public DbSet<SaveGame> SaveGames { get; set; } = default!;
        public DbSet<GameDb> Games { get; set; } = default!;
        public DbSet<GameSettingsDb> GameSettingses { get; set; } = default!;
        public DbSet<DefaultSettings> DefaultSettingses { get; set; } = default!;

        private static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .UseSqlite("Data Source=C:/Users/NZXT/RiderProjects/icd0008-2019f/ConnectF/WebApp/app.db");
        }
        //public AppDatabaseContext(DbContextOptions options) : base(options){}
    }
}