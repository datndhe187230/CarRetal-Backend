namespace HE187005_Test1_SE1841.Data;

using HE187005_Test1_SE1841.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
    }

