using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalTest24F.Data;
using FinalTest24F.Models;
using FinalTest24F.Utilites;
using FinalTest24F.Services;
using Microsoft.AspNetCore.Authorization;

namespace FinalTest24F.Controllers
{
  public class BooksController : Controller
  {
    private readonly MvcBookContext _context;
    private readonly IBook _librarian;

    public BooksController(MvcBookContext context, IBook librarian)
    {
      _context = context;
      _librarian = librarian;
    }

    // GET: Books
    public async Task<IActionResult> Index(string sortOrder)
    {
      var books = from b in _context.Books
                  select b;

      ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";

      switch (sortOrder)
      {
        case "title_desc":
          books = books.OrderByDescending(b => b.Title);
          break;
        case "title_asc":
          books = books.OrderBy(b => b.Title);
          break;
        default:
          books = books.OrderBy(b => b.Title);
          break;
      }

      return View(await books.ToListAsync());
    }
    // GET: Books/Borrow/5
    public async Task<IActionResult> Borrow(int? id)
    {
      if (id == null || !User.Identity.IsAuthenticated)
      {
        return NotFound();
      }

      var book = await _context.Books.FindAsync(id);
      if (book == null || _librarian.AvailableCopies(book.ID) <= 0)
      {
        return NotFound();
      }

      var bookLoan = new BookLoan
      {
        BookID = book.ID,
        Lender = User?.Identity?.Name,
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.Date.AddDays(14).AddHours(23).AddMinutes(59),
        Active = true
      };

      _context.BookLoans.Add(bookLoan);
      await _context.SaveChangesAsync();

      return RedirectToAction(nameof(Index));
    }
    // GET: Books/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var book = await _context.Books
          .FirstOrDefaultAsync(m => m.ID == id);
      if (book == null)
      {
        return NotFound();
      }

      return View(book);
    }

    // GET: Books/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ISBN,Title,Author,Count")] Book book)
    {
      if (ModelState.IsValid)
      {
        if (IsbnHelper.IsValidIsbn13(book.ISBN))
        {
          // Format ISBN before saving
          book.ISBN = IsbnHelper.FormatIsbn13(book.ISBN);

          // Check if ISBN already exists
          if (await _context.Books.AnyAsync(b => b.ISBN == book.ISBN))
          {
            ModelState.AddModelError("ISBN", "This ISBN already exists in the database.");
            return View(book);
          }

          _context.Add(book);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
        else
        {
          ModelState.AddModelError("ISBN", "Invalid ISBN-13 format.");
        }
      }
      return View(book);
    }

    // GET: Books/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var book = await _context.Books.FindAsync(id);
      if (book == null)
      {
        return NotFound();
      }
      return View(book);
    }

    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ID,ISBN,Title,Author,Count")] Book book)
    {
      if (id != book.ID)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        if (IsbnHelper.IsValidIsbn13(book.ISBN))
        {
          try
          {
            // Format ISBN before updating
            book.ISBN = IsbnHelper.FormatIsbn13(book.ISBN);

            // Check if ISBN already exists (excluding current book)
            if (await _context.Books.AnyAsync(b => b.ISBN == book.ISBN && b.ID != book.ID))
            {
              ModelState.AddModelError("ISBN", "This ISBN already exists in the database.");
              return View(book);
            }

            _context.Update(book);
            await _context.SaveChangesAsync();
          }
          catch (DbUpdateConcurrencyException)
          {
            if (!BookExists(book.ID))
            {
              return NotFound();
            }
            else
            {
              throw;
            }
          }
          return RedirectToAction(nameof(Index));
        }
        else
        {
          ModelState.AddModelError("ISBN", "Invalid ISBN-13 format.");
        }
      }
      return View(book);
    }

    // GET: Books/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var book = await _context.Books
          .FirstOrDefaultAsync(m => m.ID == id);
      if (book == null)
      {
        return NotFound();
      }

      return View(book);
    }

    // POST: Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var book = await _context.Books.FindAsync(id);
      if (book != null)
      {
        _context.Books.Remove(book);
      }

      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool BookExists(int id)
    {
      return _context.Books.Any(e => e.ID == id);
    }
    // --- MyLoads
    // GET: Books/MyLoans
    [Authorize]
    public async Task<IActionResult> MyLoans(bool showActive = true)
    {
      if (User.Identity?.Name == null)
        return RedirectToAction("Login", "Account");

      var query = _context.BookLoans
          .Include(bl => bl.Book)
          .Where(bl => bl.Lender == User.Identity.Name);

      if (showActive)
      {
        query = query.Where(bl => bl.Active);
      }

      var loans = await query.OrderByDescending(bl => bl.StartDate).ToListAsync();
      ViewData["ShowActiveOnly"] = showActive;

      return View(loans);
    }

    // POST: Books/Return/5
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Return(int id)
    {
      var loan = await _context.BookLoans.FindAsync(id);

      if (loan == null || loan.Lender != User.Identity?.Name)
      {
        return NotFound();
      }

      loan.Active = false;
      await _context.SaveChangesAsync();

      return RedirectToAction(nameof(MyLoans));
    }
  }
}
