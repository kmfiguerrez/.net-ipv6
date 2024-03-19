using System.Text.RegularExpressions;

public class IPv6
{

  public static bool IsValidIPv6(string ipv6Address)
  {
    // Sanitize user input first.
    ipv6Address = ipv6Address.Trim().ToLower();

    char[] ipv6Char = new char[] {
      ':', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
      'a', 'b', 'c', 'd', 'e', 'f',
    };

    // Reusable pattern.
    string regexPattern;
    // This pattern used to check valic ipv6 characters.
    const string ipv6CharPattern = @"[^a-f0-9:]";

    
    if (ipv6Address == null || ipv6Address == "")
    {
      Console.WriteLine("Invalid IPv6 format: IPv6 Address cannot be null nor an empty string.");
      return false;
    }

    // Check if user input uses valid ipv6 characters.
    if (Regex.IsMatch(ipv6Address, ipv6CharPattern))
    {
      Console.WriteLine("Invalid IPv6 format: Not valid IPv6 character(s).");
      return false;      
    }

    // IPv6 address must not start and end with a single colon.
    if ((ipv6Address[0] == ':' && ipv6Address.Substring(0, 2) != "::") || (ipv6Address[ipv6Address.Length - 1] == ':' && ipv6Address.Substring(ipv6Address.Length - 2, 2) != "::"))
    {
      Console.WriteLine("Invalid IPv6 format: Colon used at the beginning or end at the end.");
      return false;
    }

    // IPv6 address cannot have more than two contiguous colons.
    regexPattern = @":::";
    if (Regex.IsMatch(ipv6Address, regexPattern))
    {
      Console.WriteLine("Invalid IPv6 format: Colon used more than twice contiguously.");
      return false;
    }

    // IPv6 address should have only one double-colon in use.
    regexPattern = @"::";
    MatchCollection dcInstance = Regex.Matches(ipv6Address, regexPattern);
    if (dcInstance.Count > 1)
    {
      Console.WriteLine("Invalid IPv6 format: Double-colon used more than once.");
      return false;      
    }

    // Utils.PrintCollection(dcInstance.ToList());
    
    
    return true;
  }
}