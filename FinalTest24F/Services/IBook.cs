namespace FinalTest24F.Services
{
  public interface IBook
  {
    public string BookAvailableOn(int bookID);
    public int OverDueBooks(int bookID);
    public int AvailableCopies(int bookID);
  }
}
