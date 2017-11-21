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
    internal class RoomStoreContext : DbContext
    {
        public DbSet<EventRecord> Events { get; set; }
        public DbSet<EventEdgeRecord> EventEdges { get; set; }
        public DbSet<EventHashRecord> EventHashes { get; set; }
        public RoomStoreContext() : base()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(DatastoreService.DBNAME);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
