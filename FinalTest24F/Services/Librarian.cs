using FinalTest24F.Data;
using FinalTest24F.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalTest24F.Services
{
  public class Librarian : IBook
  {
    private readonly MvcBookContext _context;

    public Librarian(MvcBookContext context)
    {
      _context = context;
    }
    public int AvailableCopies(int bookID)
    {
      var book = _context.Books.Find(bookID);
      if (book == null) return 0;

      var activeLoans = _context.BookLoans
          .Count(bl => bl.BookID == bookID && bl.Active);

      var availableCopies = book.Count - activeLoans;
      return availableCopies;
    }

    public string BookAvailableOn(int bookID)
    {
      var availableCopies = AvailableCopies(bookID);

      if (availableCopies > 0)
        return "Available";

      var earliestReturn = _context.BookLoans
          .Where(bl => bl.BookID == bookID && bl.Active)
          .OrderBy(bl => bl.EndDate)
          .Select(bl => bl.EndDate)
          .FirstOrDefault();

      return earliestReturn != default ? earliestReturn.ToString("MM-dd-yyyy") : "No copies available";
    }

    public int OverDueBooks(int bookID)
    {
      return _context.BookLoans
          .Count(bl => bl.BookID == bookID &&
                      bl.Active &&
                      bl.EndDate < DateTime.Today);
    }

    public IEnumerable<Book> MyBookLoans(string userEmail)
    {
      return _context.BookLoans
            .Where(bl => bl.Lender == userEmail)
            .Include(bl => bl.Book)  // Include the Book navigation property
            .Select(bl => bl.Book!)  // Use the null-forgiving operator since we know Book exists
            .Where(b => b != null)   // Filter out any null books just to be safe
            .Distinct()
            .ToList();
    }
  }
}