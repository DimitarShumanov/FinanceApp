using FinanceApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FinanceApp.Controllers
{
    // Change base class from Controller to IdentityDbContext or DbContext
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Models.Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // seed predefined categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Заплата", IsIncome = true, UserId = null },
                new Category { Id = 2, Name = "Бонус", IsIncome = true, UserId = null },
                new Category { Id = 10, Name = "Храна", IsIncome = false, UserId = null },
                new Category { Id = 11, Name = "Транспорт", IsIncome = false, UserId = null },
                new Category { Id = 12, Name = "Махане", IsIncome = false, UserId = null }
            );
        }
    }
}
