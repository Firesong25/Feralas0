using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Feralas
{
    public partial class sagorneContext : DbContext
    {
        public sagorneContext()
        {
        }

        public sagorneContext(DbContextOptions<sagorneContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Holding> Holdings { get; set; }
        public virtual DbSet<StockQuote> Stockquotes { get; set; }

        public string ConnectionString = string.Empty;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Holding>(entity =>
            {
                entity.ToTable("holdings");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Day).HasColumnName("day");

                entity.Property(e => e.Epic)
                    .IsRequired()
                    .HasColumnName("epic")
                    .HasMaxLength(10);

                entity.Property(e => e.Month).HasColumnName("month");

                entity.Property(e => e.PeakPrice).HasColumnName("peak_price");

                entity.Property(e => e.PricePaid).HasColumnName("price_paid");

                entity.Property(e => e.ProcessId).HasColumnName("process_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.SoldPrice).HasColumnName("sold_price");

                entity.Property(e => e.TimeBought).HasColumnName("time_bought");

                entity.Property(e => e.TimeSold).HasColumnName("time_sold");

                entity.Property(e => e.TimeToSell).HasColumnName("time_to_sell");

                entity.Property(e => e.Year).HasColumnName("year");
            });


            modelBuilder.Entity<StockQuote>(entity =>
            {
                entity.ToTable("stockquotes");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Ask).HasColumnName("ask");

                entity.Property(e => e.Bid).HasColumnName("bid");

                entity.Property(e => e.Day).HasColumnName("day");

                entity.Property(e => e.Epic)
                    .IsRequired()
                    .HasColumnName("epic")
                    .HasMaxLength(10);

                entity.Property(e => e.EpochTime).HasColumnName("epoch_time");

                entity.Property(e => e.Month).HasColumnName("month");

                entity.Property(e => e.Timestamp).HasColumnName("timestamp");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
