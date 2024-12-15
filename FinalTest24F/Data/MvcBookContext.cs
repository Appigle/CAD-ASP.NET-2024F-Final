using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinalTest24F.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FinalTest24F.Data
{
  public class MvcBookContext : IdentityDbContext<IdentityUser>
  {
    public MvcBookContext(DbContextOptions<MvcBookContext> options)
        : base(options)
    {
    }

    public DbSet<FinalTest24F.Models.Book> Books { get; set; } = default!;
    public DbSet<FinalTest24F.Models.BookLoan> BookLoans { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      builder.Entity<BookLoan>()
            .HasOne(bl => bl.Book)
            .WithMany(b => b.BookLoans)
            .HasForeignKey(bl => bl.BookID)
            .OnDelete(DeleteBehavior.Restrict);
      builder.Entity<Book>().HasData(
          new Book { ID = 1, ISBN = "978-0-43902-348-1", Title = "The Hunger Games", Author = "Suzanne Collins", Count = 2 },
          new Book { ID = 2, ISBN = "978-1-98217-686-0", Title = "Steve Jobs", Author = "Catmull Wallace", Count = 2 },
          new Book { ID = 3, ISBN = "978-0-69116-472-4", Title = "Alan Turing: The Enigma", Author = "Andrew Hodges", Count = 1 },
          new Book { ID = 4, ISBN = "978-1-52476-316-9", Title = "A Promised Land", Author = "Barack Hussain Obama", Count = 4 },
          new Book { ID = 5, ISBN = "978-0-06231-500-7", Title = "The Alchemist", Author = "Paulo Coelho", Count = 3 },
          new Book { ID = 6, ISBN = "979-8-66994-364-6", Title = "Les Miserables", Author = "Victor Hugo", Count = 1 }
      );
    }
  }
}
