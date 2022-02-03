using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebAPI.Model;
using WebAPI.Model.Entity;

namespace WebAPI.Model
{
    public partial class DB_Context : DbContext
    {
        public DB_Context(DbContextOptions options)
            : base(options)
        {
        }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<EVoucher> EVoucher { get; set; }
        public virtual DbSet<Purchase> Purchase { get; set; }
        public virtual DbSet<Transcation> Transcation { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethod { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");
            });
            modelBuilder.Entity<EVoucher>(entity =>
            {
                entity.ToTable("evoucher");
            });
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.ToTable("purchase");
            });
            modelBuilder.Entity<Transcation>(entity =>
            {
                entity.ToTable("transcation");
            });
            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("payment_method");
            });
        }

    }
}
