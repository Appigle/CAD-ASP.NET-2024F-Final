using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalTest24F.Models
{
  public class BookLoan
  {
    [Key]
    public int ID { get; set; }

    [Required]
    [Display(Name = "Borrower")]
    public string Lender { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Borrow Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required]
    public bool Active { get; set; }

    // Foreign key for Book
    [Required]
    public int BookID { get; set; }

    // Navigation property
    public Book? Book { get; set; }


  }
}