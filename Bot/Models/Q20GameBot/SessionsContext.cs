using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Bot.Q20GameBot.Models
{
    public partial class SessionsContext : DbContext
    {
        public SessionsContext(DbContextOptions<SessionsContext> context)
            : base(context)
        {

        }

        public virtual DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.ChatId);

                entity.Property(e => e.ChatId).ValueGeneratedNever();

                entity.Property(e => e.LastRequestMade).HasColumnType("datetime");
            });
        }
    }
}
