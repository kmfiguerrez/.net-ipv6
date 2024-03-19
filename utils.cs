using System.Text.RegularExpressions;


public class Utils
{
  public static void PrintCollection<T>(List<T> collection)
  {
    string display = "";
    Regex regexPattern = new(@", $");
    
    display += '[';

    foreach (var item in collection)
    {
      if (item != null)
      {
        display += item.ToString() + ',' + ' ';
      }
    }

    // Remove trailing comma and a single white space
    // and replace it with a closing square bracket.
    display = regexPattern.Replace(display, "]");

    Console.WriteLine(display);
  }
}