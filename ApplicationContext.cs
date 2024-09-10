using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegistrationAuthorization;

namespace RegistrationAuthorization
{
    public class ApplicationContext : DbContext
    {

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<User> Users => Set<User>();


        public ApplicationContext() 
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated(); }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = login.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithOne()  // нет обратной навигации, если не нужно
                .HasForeignKey<User>(u => u.AccountId);  // внешний ключ в User
        }

    }
}
