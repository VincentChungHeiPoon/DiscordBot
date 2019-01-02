using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace DiscordBot.Resources.Database
{
    public class SqliteDbContext : DbContext
    {
        //test run
        public DbSet<Stone> Stones { get; set; }

        //project management tables
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectFile> Files { get; set; }
        public DbSet<Group> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder Options)
        {
            string DtLocation = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1", @"Data");
            Options.UseSqlite($"Data Source={DtLocation}Database.sqlite");
        }
    }
}
