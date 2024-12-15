namespace FinalTest24F.Utilites
{
  public static class IsbnHelper
  {
    public static string FormatIsbn13(string isbn)
    {
      // Remove any existing hyphens
      string cleanIsbn = isbn.Replace("-", "");

      if (cleanIsbn.Length != 13)
        return isbn; // Return original if not 13 digits

      // Format into groups
      return $"{cleanIsbn.Substring(0, 3)}-{cleanIsbn.Substring(3, 1)}-" +
             $"{cleanIsbn.Substring(4, 5)}-{cleanIsbn.Substring(9, 3)}-{cleanIsbn.Substring(12, 1)}";
    }

    public static bool IsValidIsbn13(string isbn)
    {
      string cleanIsbn = isbn.Replace("-", "");

      // Check if it starts with valid prefix
      if (!cleanIsbn.StartsWith("978") && !cleanIsbn.StartsWith("979") && !cleanIsbn.StartsWith("989"))
        return false;

      // Check length
      if (cleanIsbn.Length != 13)
        return false;

      // Check if all characters are digits
      if (!cleanIsbn.All(char.IsDigit))
        return false;

      return true;
    }
  }
}