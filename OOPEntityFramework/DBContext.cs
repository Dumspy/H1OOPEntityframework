using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using OOPEntityFramework.Classes;

public class DBContext : DbContext
{
    public DbSet<Person> People { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    private string DbPath { get; }

    public DBContext()
    {
        var folder = Environment.SpecialFolder.Desktop;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "database.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}