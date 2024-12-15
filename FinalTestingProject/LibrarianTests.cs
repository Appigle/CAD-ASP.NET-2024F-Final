using Microsoft.EntityFrameworkCore;
using FinalTest24F.Data;
using FinalTest24F.Models;
using FinalTest24F.Services;
using Xunit;

namespace FinalTest24F.UnitTests;
public class LibrarianTests : IDisposable
{
  private readonly MvcBookContext _context;
  private readonly IBook _librarian;

  public LibrarianTests()
  {
    var options = new DbContextOptionsBuilder<MvcBookContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    _context = new MvcBookContext(options);
    _librarian = new Librarian(_context);
    SeedTestData();
  }

  private void SeedTestData()
  {
    // Add test books
    var books = new List<Book>
        {
            new Book { ID = 1, ISBN = "978-1-234", Title = "Test Book 1", Author = "Author 1", Count = 2 },
            new Book { ID = 2, ISBN = "978-5-678", Title = "Test Book 2", Author = "Author 2", Count = 1 }
        };
    _context.Books.AddRange(books);

    // Add test loans
    var loans = new List<BookLoan>
        {
            new BookLoan
            {
                ID = 1,
                BookID = 1,
                Lender = "user1@test.com",
                StartDate = DateTime.Today.AddDays(-10),
                EndDate = DateTime.Today.AddDays(-1),
                Active = true
            },
            new BookLoan
            {
                ID = 2,
                BookID = 1,
                Lender = "user2@test.com",
                StartDate = DateTime.Today.AddDays(-5),
                EndDate = DateTime.Today.AddDays(5),
                Active = true
            }
        };
    _context.BookLoans.AddRange(loans);
    _context.SaveChanges();
  }

  [Fact]
  public void BookAvailableOn_WithAvailableCopies_ReturnsAvailable()
  {
    // Add debug output
    var book = _context.Books.Find(1);
    var activeLoans = _context.BookLoans.Count(bl => bl.BookID == 1 && bl.Active);

    var result = _librarian.BookAvailableOn(1);
    Assert.Equal("12-13-2024", result);
  }

  [Fact]
  public void BookAvailableOn_WithNoAvailableCopies_ReturnsEarliestDate()
  {
    // Add another active loan for Book 1
    var loan = new BookLoan
    {
      ID = 3,
      BookID = 1,
      Lender = "user3@test.com",
      StartDate = DateTime.Today,
      EndDate = DateTime.Today.AddDays(3),
      Active = true
    };
    _context.BookLoans.Add(loan);
    _context.SaveChanges();

    var result = _librarian.BookAvailableOn(1);
    Assert.Equal(DateTime.Today.AddDays(-1).ToString("MM-dd-yyyy"), result);
  }

  [Fact]
  public void OverDueBooks_WithOverdueLoans_ReturnsCorrectCount()
  {
    // Book 1 has one overdue loan
    var result = _librarian.OverDueBooks(1);
    Assert.Equal(1, result);
  }

  [Fact]
  public void OverDueBooks_WithNoOverdueLoans_ReturnsZero()
  {
    // Add book with no overdue loans
    var book = new Book { ID = 3, ISBN = "978-9-012", Title = "Test Book 3", Author = "Author 3", Count = 1 };
    _context.Books.Add(book);

    var loan = new BookLoan
    {
      ID = 4,
      BookID = 3,
      Lender = "user4@test.com",
      StartDate = DateTime.Today,
      EndDate = DateTime.Today.AddDays(7),
      Active = true
    };
    _context.BookLoans.Add(loan);
    _context.SaveChanges();

    var result = _librarian.OverDueBooks(3);
    Assert.Equal(0, result);
  }

  [Fact]
  public void BookAvailableOn_WithNonexistentBook_ReturnsNoCopiesAvailable()
  {
    var result = _librarian.BookAvailableOn(999);
    Assert.Equal("No copies available", result);
  }

  [Fact]
  public void OverDueBooks_WithNonexistentBook_ReturnsZero()
  {
    var result = _librarian.OverDueBooks(999);
    Assert.Equal(0, result);
  }

  public void Dispose()
  {
    _context.Database.EnsureDeleted();
    _context.Dispose();
  }
}