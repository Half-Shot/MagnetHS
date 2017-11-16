using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.DatastoreService.Records;
using HalfShot.MagnetHS.CommonStructures.Events;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
namespace HalfShot.MagnetHS.DatastoreService.Contexts
{
    internal class UserStoreContext : DbContext
    {
        public DbSet<UserRecord> Users { get; set; }
        public DbSet<ProfileRecord> Profiles { get; set; }
        public DbSet<PasswordRecord> Passwords { get; set; }
        public UserStoreContext() : base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(DatastoreService.DBNAME);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProfileRecord>().HasKey(c => new { c.UserId, c.Key });
            modelBuilder.Entity<UserRecord>().HasIndex(c => new { c.UserId });
            modelBuilder.Entity<PasswordRecord>().HasIndex(c => new { c.UserId });
            /*var content = modelBuilder.Entity<BaseEvent>().Property(c => new { c.Content });
            content.HasValueGenerator((prop, type) =>
            {
                prop.
            });*/

        }
    }
}
