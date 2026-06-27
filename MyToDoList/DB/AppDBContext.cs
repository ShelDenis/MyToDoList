using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MyToDoList.Models;

namespace MyToDoList.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public DbSet<Task> Tasks { get; set; } = null!;
        public DbSet<TaskGroup> TaskGroups { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "tasksdb.db");

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlite($"Data Source={dbPath}")
                    .LogTo(message => Console.WriteLine($"[SQL] {message}")); 
            }
        }
    }
}
