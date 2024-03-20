using System.Text.RegularExpressions;

public struct IPv6ReturnData
{
  public bool success;
  public string? data;
  public string? message;

}

public class IPv6
{

  /// <summary>
  /// This method is used to check the format of IPv6 address.
  /// </summary>
  /// <param name="ipv6Address"></param>
  /// <returns>Boolean.</returns>
  public static bool IsValidIPv6(string ipv6Address)
  {
    // Sanitize user input first.
    ipv6Address = ipv6Address.Trim().ToLower();

    // Regex Pattern.
    // [0-9a-f]{1,4} means a segment of at least one and max at four of hex digits.
    // [0-9a-f]{1,4}: means a segment that ends with a semi-colon.
    // ([0-9a-f]{1,4}:){7} means a segment ends with a semi-colon, seven times.
    string completeIPv6FormatPattern = @"^([a-f0-9]:{1,4}){7}[a-f0-9]{1,4}$";
    string regexPattern;
    // This pattern used to check valic ipv6 characters.
    const string ipv6CharPattern = @"[^a-f0-9:]";

    MatchCollection segments;


    
    if (ipv6Address == null || ipv6Address == "")
    {
      Console.WriteLine("From IsValidIPv6: Invalid IPv6 format - IPv6 Address cannot be null nor an empty string.");
      return false;
    }

    // Check if user input uses valid ipv6 characters.
    if (Regex.IsMatch(ipv6Address, ipv6CharPattern))
    {
      Console.WriteLine("From IsValidIPv6: Invalid IPv6 format - Not valid IPv6 character(s).");
      return false;      
    }

    // IPv6 address must not start and end with a single colon.
    if ((ipv6Address[0] == ':' && ipv6Address.Substring(0, 2) != "::") || (ipv6Address[ipv6Address.Length - 1] == ':' && ipv6Address.Substring(ipv6Address.Length - 2, 2) != "::"))
    {
      Console.WriteLine("From IsValidIPv6: Invalid IPv6 format - Colon used at the beginning or end at the end.");
      return false;
    }

    // IPv6 address cannot have more than two contiguous colons.
    regexPattern = @":::";
    if (Regex.IsMatch(ipv6Address, regexPattern))
    {
      Console.WriteLine("From IsValidIPv6: Invalid IPv6 format - Colon used more than twice contiguously.");
      return false;
    }

    // IPv6 address should have only one double-colon in use.
    regexPattern = @"::";
    MatchCollection dcInstance = Regex.Matches(ipv6Address, regexPattern);
    if (dcInstance.Count > 1)
    {
      Console.WriteLine("From IsValidIPv6: Invalid IPv6 format - Double-colon used more than once.");
      return false;      
    }

    // IPv6 address should only have a max four hex digits in each segment.
    regexPattern = @"[0-9a-f]{1,}";
    segments = Regex.Matches(ipv6Address, regexPattern);
    if (segments.Count != 0)
    {
      foreach (Match segment in segments)
      {
        if (segment.Value.Length > 4)
        {
          Console.WriteLine("From IsValidIPv6: Invalid IPv6 format - A segment can only have a max of four hex digits.");
          return false;           
        }
      }
    }

    // If double colon is not used then a valid ipv6 address has an eight groups of segments and should only have a max of 7 colons.
    // Otherwise used, then double-colon can only be used if there's two or more consecutive of segments of all zeros.
    // So segments cannot be more than six.
    regexPattern = @"::";
    if (!Regex.IsMatch(ipv6Address, regexPattern) && !Regex.IsMatch(ipv6Address, completeIPv6FormatPattern))
    {
      Console.WriteLine("From IsValidIPv6: Invalid IPv6 format - A segment can only have a max of four hex digits.");
      return false;       
    }
    else
    {
      // Otherwise :: is used.
      segments = Regex.Matches(ipv6Address, @"[0-9a-f]{1,4}");
      if (segments.Count > 6)
      {
        Console.WriteLine("From IsValidIPv6: Invalid IPv6 format - IPv6 Address with double-colon were not used in proper format.");
        return false;         
      }
    }

    // Finally return true if all checkings passed.
    Console.WriteLine("Valid IPv6 Address");
    return true;
  }




















}