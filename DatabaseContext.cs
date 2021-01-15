using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using BigMoBot.Database.FunctionModels;
using System.IO;
using System.Reflection;

namespace BigMoBot.Database
{

    public class DatabaseContext : DatabaseEntities
    {
        public DatabaseContext() : base()
        {}

        public DbSet<UserTotalMessages> UserTotalMessagesModel { get; set; }
        public DbSet<UserTotalVoiceTime> UserTotalVoiceTimeModel { get; set; }
        public DbSet<ChannelTotalMessages> ChannelTotalMessagesModel { get; set; }
        public DbSet<UserActivity> UserActivityModel { get; set; }
        public DbSet<LastLogs> LastLogsModel { get; set; }
        public DbSet<HelloMessageCount> HelloMessageCountModel { get; set; }
        public DbSet<IterationCount> IterationCountModel { get; set; }
        public DbSet<ChainBreakCount> ChainBreakCountModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserTotalMessages>().HasNoKey();
            modelBuilder.Entity<UserTotalVoiceTime>().HasNoKey();
            modelBuilder.Entity<ChannelTotalMessages>().HasNoKey();
            modelBuilder.Entity<UserActivity>().HasNoKey();
            modelBuilder.Entity<LastLogs>().HasNoKey();
            modelBuilder.Entity<HelloMessageCount>().HasNoKey();
            modelBuilder.Entity<IterationCount>().HasNoKey();
            modelBuilder.Entity<ChainBreakCount>().HasNoKey();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if (DEBUG)
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=BigMoBot;Trusted_Connection=True;", sso => sso.MaxBatchSize(128));
#else
            var assembly = Assembly.GetExecutingAssembly();
            var DbStringFile = "BigMoBot.Data.DbString.txt";

            using (Stream stream = assembly.GetManifestResourceStream(DbStringFile))
            using (StreamReader reader = new StreamReader(stream))
                optionsBuilder.UseSqlServer(reader.ReadToEnd().Trim(), sso => sso.MaxBatchSize(128));
#endif
        }
    }
}
