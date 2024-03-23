using System.Text.RegularExpressions;

public struct IPv6ReturnData
{
  public bool success;
  public string? data;
  public string? error;

}

/*
  In this class definitions. I decided to use hexadecimals and binaries 
  as type string because:
  - Hex includes letters.
  - Data coming from user interfaces are of type string.
*/
public class IPv6
{

  /// <summary>
  /// This method is used to check the format of IPv6 address.
  /// </summary>
  /// <param name="ipv6Address">An IPv6 addresss of type string.</param>
  /// <returns>Boolean.</returns>
  public static bool IsValidIPv6(string ipv6Address)
  {
    // Sanitize user input first.
    ipv6Address = ipv6Address.Trim().ToLower();

    // Regex Pattern.
    // [0-9a-f]{1,4} means a segment of at least one and max at four of hex digits.
    // [0-9a-f]{1,4}: means a segment that ends with a semi-colon.
    // ([0-9a-f]{1,4}:){7} means a segment ends with a semi-colon, seven times.
    string completeIPv6FormatPattern = @"([a-f0-9]{1,4}:){7}[a-f0-9]{1,4}";
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
    regexPattern = @"::";
    if (!Regex.IsMatch(ipv6Address, regexPattern) && !Regex.IsMatch(ipv6Address, completeIPv6FormatPattern))
    {
      Console.WriteLine("From IsValidIPv6: Invalid IPv6 format - Invalid IPv6 address provided.");
      return false;       
    }
    
    // If then double-colon can only be used if there's two or more consecutive of segments of all zeros.
    // So segments cannot be more than six.
    if (Regex.IsMatch(ipv6Address, regexPattern))
    {
      segments = Regex.Matches(ipv6Address, @"[0-9a-f]{1,4}");
      if (segments.Count > 6)
      {
        Console.WriteLine("From IsValidIPv6: Invalid IPv6 format - IPv6 Address with double-colon were not used in proper format.");
        return false;         
      }
    }

    // Finally return true if all checkings passed.
    // Console.WriteLine("Valid IPv6 Address");
    return true;
  }


  /// <summary>
  /// This method expands an abbreviated IPv6 address
  /// by adding leading zeroes to segment and turning
  /// :: into a series of segment of zeroes.
  /// </summary>
  /// <param name="ipv6Address">An IPv6 address of type string.</param>
  /// <returns>
  /// An object that has three properties: success and two nullable 
  /// string type: error and data.
  /// </returns>
  public static IPv6ReturnData Expand(string ipv6Address)
  {
    // Sanitize user input first.
    ipv6Address = ipv6Address.Trim().ToLower();

    // Regex pattern.
    string doubleColonPattern = @"^::$";
    string segmentPattern = @"[0-9a-f]{1,4}";

    List<string> segments = [];

    // Return data.
    IPv6ReturnData expandedIPv6;


    try
    {
      // Check first if the address is valid.
      if (!IsValidIPv6(ipv6Address)) throw new ArgumentException("From Expand: Invalid IPv6 provided.");
    }
    catch (ArgumentException ex)
    {
      expandedIPv6.success = false;
      expandedIPv6.error = ex.Message;
      expandedIPv6.data = null;
      return expandedIPv6;
    }

    // Check if the user input is just :: 
    if (Regex.IsMatch(ipv6Address, doubleColonPattern))
    {
      expandedIPv6.success = true;
      expandedIPv6.data = "0000:0000:0000:0000:0000:0000:0000:0000";
      expandedIPv6.error = null;
      return expandedIPv6;
    }

    // Add leading zeros if it has to.
    segments = Regex.Matches(ipv6Address, segmentPattern).Select(match => match.Value).ToList();
    for (int i = 0; i < segments.Count; i++)
    {
      string segment = segments[i];
      if (segment.Length != 4)
      {
        int zeroesToPrepend = 4 - segment.Length;
        string leadingZeroes = String.Concat(Enumerable.Repeat('0', zeroesToPrepend));
        segments[i] = leadingZeroes + segment;        
      }
    }

    // Turn double colon(::) into segments of zeros.
    // Check if double colon(::) occurs at the end.
    if (ipv6Address.Substring(ipv6Address.Length - 2, 2) == "::")
    {
      // Append segment of zeros until there's eight segments.
      while (segments.Count != 8)
      {
        segments.Add("0000");
      }
    }
    else
    {
      // Otherwise double colon(::) occurs somewhere not at the end.
      // Find the index of the double-colon(::)
      int toInsertAt = Array.IndexOf(ipv6Address.Split(':'), "");
      // Keep adding until there's a total of 8 segments.
      while (segments.Count != 8)
      {
        segments.Insert(toInsertAt, "0000");
      }
    }

    // Update message.
    expandedIPv6.success = true;
    expandedIPv6.error = null;
    expandedIPv6.data = String.Join(':', segments);    
    // Finally
    return expandedIPv6;
  }


  /// <summary>
  /// This method abbreviates an IPv6 address by removing leading zeroes
  /// and turning a series of segments of zeroes.
  /// </summary>
  /// <param name="ipv6Address"></param>
  /// <returns>
  /// An object that has three properties: success and two nullable 
  /// string type: error and data.
  /// </returns>
  public static IPv6ReturnData Abbreviate(string ipv6Address)
  {
    // Sanitize user input first.
    ipv6Address = ipv6Address.Trim().ToLower();

    // Regex Pattern
    const string segmentAllZeroPattern = @"^0000$";
    const string leadingZeroPattern = @"^0+";
    // This pattern assumes the IPv6 Address has no leading zeros.
    const string seriesOfZeroesPattern = @"0(:0){1,}";

    string[] segments;

    // Return data.
    IPv6ReturnData abbreviatedIPv6;


    // Try to expand the address first
    try
    {
      // Validate the address first.
      if (!IsValidIPv6(ipv6Address)) throw new ArgumentException("From Abbreviate: Provided invalid IPv6 address.");

      IPv6ReturnData expandedIPv6 = Expand(ipv6Address);
      if (!expandedIPv6.success) throw new ArgumentException("From Abbreviate: Expanding Part Failed.");

      // Set the segments.
      // if (expandedIPv6.data != null)
      // {
      //   segments = expandedIPv6.data.Split(':');
      // }
      // else
      // {
      //   throw new ArgumentException("From Abbreviate: Coudn't abbreviate the address.");
      // }

      segments = expandedIPv6.data.Split(':');

      // Remove leading zeroes.
      for (int i = 0; i < segments.Length; i++)
      {
        // Turn 0000 into 0.
        if (Regex.IsMatch(segments[i], segmentAllZeroPattern))
        {
          segments[i] = "0";
          continue;
        }
        
        segments[i] = Regex.Replace(segments[i], leadingZeroPattern, "");
      }

      string ipv6Str = String.Join(':', segments);

      // Get the instances of series of segments of zeroes if exist.
      string[] instances = Regex.Matches(ipv6Str, seriesOfZeroesPattern).Select(match => match.Value).ToArray();
      if (instances.Length != 0)
      {
        // Choose the longest sequence.
        // Set the temporary longest sequence.
        string longestSequence = instances[0];
        // Update the longest sequence.
        foreach (string instance in instances)
        {
          if (instance.Length > longestSequence.Length) longestSequence = instance;
        }

        // Turn the longest sequence into double-colon (::)
        // Update the data.
        abbreviatedIPv6.data = Regex.Replace(ipv6Str, longestSequence, "::");
        // The replace method above causes more than two of contiguous colons.
        // So perform a replace again.
        abbreviatedIPv6.data = Regex.Replace(abbreviatedIPv6.data, @":{3,}", "::");
      }
      else
      {
        // Otherwise none
        abbreviatedIPv6.data = ipv6Str;
      }

    }
    catch (ArgumentException ex)
    {
      abbreviatedIPv6.success = false;
      abbreviatedIPv6.data = null;
      abbreviatedIPv6.error = ex.Message;
      return abbreviatedIPv6;
    }

    // Update the return data.
    abbreviatedIPv6.success = true;
    abbreviatedIPv6.error = null;
    // Finally. 
    return abbreviatedIPv6;
  }


  /// <summary>
  /// This method validates whether the input strings a hex or not.
  /// </summary>
  /// <param name="hex">A string of hex digits.</param>
  /// <returns>Boolean</returns>
  public static bool IsHex(string hex)
  {
    // Sanitize input data first.
    hex = hex.Trim().ToLower();

    // Regex pattern
    const string hexCharsPattern = @"[^0-9a-f]";

    bool invalidHex;


    // Validate input data first.
    if (hex == null || hex == "") return false;

    invalidHex = Regex.IsMatch(hex, hexCharsPattern);
    if (invalidHex) return false;

    // Finally
    return true;
  }


  /// <summary>
  /// This method validates if the input binary string a hex or not.
  /// 
  /// </summary>
  /// <param name="binary">A string of binaries.</param>
  /// <returns>Boolean</returns>
  public static bool IsBinary(string binary)
  {
    // Sanitize input data first.
    binary = binary.Trim().ToLower();

    // Regex pattern
    const string binaryCharsPattern = @"[^0-1]";

    bool invalidBinary;


    // Validates input data first.
    if (binary == null || binary == "") return false;

    invalidBinary = Regex.IsMatch(binary, binaryCharsPattern);
    if (invalidBinary) return false;

    // Finally
    return true;
  }


















}