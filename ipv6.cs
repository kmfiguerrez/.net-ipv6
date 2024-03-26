using System.Numerics;
using System.Text.RegularExpressions;


/*
  In this class definitions. I decided to use hexadecimals and binaries 
  as type string because:
  - Hex includes letters.
  - Data coming from user interfaces are of type string.

  Exceptions to above statements.
  - Hexadecimals as argument to ToDecimal will be an integral type.
*/
internal static class IPv6
{

  /// <summary>
  /// This method is used to validate the format of IPv6 address.
  /// </summary>
  /// <param name="ipv6Address">An IPv6 addresss of type string.</param>
  /// <returns>Boolean.</returns>
  internal static bool IsValidIPv6(string ipv6Address)
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
  /// 
  /// <remarks>
  /// Throws an exception if fails to complete.
  /// </remarks>
  /// 
  /// <param name="ipv6Address">An IPv6 address of type string.</param>
  /// 
  /// <returns>
  /// Expanded IPv6 address
  /// </returns>
  /// 
  /// <exception cref="ArgumentException">
  /// Throws an exception if the input IPv6 address is not valid.
  /// </exception>
  internal static string Expand(string ipv6Address)
  {
    // Sanitize user input first.
    ipv6Address = ipv6Address.Trim().ToLower();

    // Regex pattern.
    string doubleColonPattern = @"^::$";
    string segmentPattern = @"[0-9a-f]{1,4}";

    List<string> segments = [];

    // Return data.
    string expandedIPv6;


    // First validate input data.
    if (!IsValidIPv6(ipv6Address)) throw new ArgumentException("From Expand: Invalid IPv6 provided.");

    // Check if the user input is just :: 
    if (Regex.IsMatch(ipv6Address, doubleColonPattern))
    {
      expandedIPv6 = "0000:0000:0000:0000:0000:0000:0000:0000";
      return expandedIPv6;
    }

    // Add leading zeros if it has to.
    segments = Regex.Matches(ipv6Address, segmentPattern).Select(match => match.Value).ToList();
    for (int i = 0; i < segments.Count; i++)
    {
      string segment = segments[i];
      if (segment.Length != 4)
      {
        segments[i] = segment.PadLeft(4, '0');        
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

    // Update return data.
    expandedIPv6 = String.Join(':', segments);    
    // Finally
    return expandedIPv6;
  }


  /// <summary>
  /// This method abbreviates an IPv6 address by removing leading zeroes
  /// and turning a series of segments of zeroes.
  /// </summary>
  /// 
  /// <remarks>
  /// Throws an exception if fails to complete.
  /// </remarks>
  /// 
  /// <param name="ipv6Address"></param>
  /// 
  /// <returns>
  /// An object that has three properties: success and two nullable 
  /// string type: error and data.
  /// </returns>
  /// 
  /// <exception cref="ArgumentException">
  /// Throws an exception if the input IPv6 address is not valid.
  /// </exception>
  internal static string Abbreviate(string ipv6Address)
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
    string abbreviatedIPv6;


    // Validate the input address first.
    if (!IsValidIPv6(ipv6Address)) throw new ArgumentException("From Abbreviate: Provided invalid IPv6 address.");
    
    // Try to expand the address first
    try
    {
      string expandedIPv6 = Expand(ipv6Address);

      segments = expandedIPv6.Split(':');

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

      // Combine the segments into a single string as IPv6 address.
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
        abbreviatedIPv6 = Regex.Replace(ipv6Str, longestSequence, "::");
        // The replace method above causes more than two of contiguous colons.
        // So perform a replace again.
        abbreviatedIPv6 = Regex.Replace(abbreviatedIPv6, @":{3,}", "::");
      }
      else
      {
        // Otherwise none
        abbreviatedIPv6 = ipv6Str;
      }
    }
    catch (ArgumentException)
    {
      throw new ArgumentException("From Abbreviate: Expanding Part Failed.");
    }

    // Finally. 
    return abbreviatedIPv6;
  }


  /// <summary>
  /// This method validates whether the input strings a hex or not.
  /// </summary>
  /// <param name="hex">A string of hex digits.</param>
  /// <returns>Boolean</returns>
  internal static bool IsHex(string hex)
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
  internal static bool IsBinary(string binary)
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


  /// <summary>
  /// This method converts positive hexadecimals to binary.
  /// </summary>
  /// 
  /// <remarks>
  /// Throws an exception if fails to complete.
  /// </remarks>
  /// 
  /// <param name="hexadecimals">A string positive hex digits.</param>
  /// 
  /// <returns>
  /// A string of binaries
  /// </returns>
  /// 
  /// <exception cref="ArgumentException">
  /// Throws an exception if the input hexadecimals is not valid.
  /// </exception>  
  internal static string ToBinary(string hexadecimals)
  {
    // Sanitize input data first.
    hexadecimals = hexadecimals.Trim().ToLower();
    // Return data.
    string binaries = "";


    // Validate input data first.
    if (!IsHex(hexadecimals)) throw new ArgumentException("From ToBinary: Invalid Hex digits provided.");

    /*
      Because the value (2 ** 55) lose precision we have to convert
      individual hex from input if multiple hex are given rather than
      the whole hexadecimals in one go.        
    */
    foreach (char hex in hexadecimals)
    {
      // First, convert hex to integer.
      // byte is an unsigned integer that can hold values from 0 to 255.
      byte integer = Convert.ToByte(hex.ToString(), 16);

      // Then convert from integer to binary.
      string binary = Convert.ToString(integer, 2);

      // Because toString method does not add leading zeros
      // we have to prepend leading zeros.
      binaries += binary.PadLeft(4, '0');
    }

    // Finally.
    return binaries;
  }


  /// <summary>
  /// Converts positive integers into binary.
  /// </summary>
  /// 
  /// <param name="integer">A positive integers range from 0 to 255.</param>
  /// 
  /// <returns>
  /// A string of binaries
  /// </returns>
  internal static string ToBinary(byte integer)
  {
    // Return data
    string binaries;


    // Convert integer to binary.
    binaries = Convert.ToString(integer, 2);

    // Finally.
    return binaries;
  }


  /// <summary>
  /// Converts positive integers into binary.
  /// </summary>
  /// 
  /// <param name="integer">A positive integers (range from 0 to 65,535).</param>
  /// 
  /// <returns>
  /// A string of binaries
  /// </returns>
  internal static string ToBinary(ushort integer)
  {
    // Return data
    string binaries;


    // Convert integer to binary.
    binaries = Convert.ToString(integer, 2);

    // Finally.
    return binaries;
  }


  /// <summary>
  /// Converts positive integers into binary.
  /// </summary>
  /// 
  /// <param name="integer">A positive integers (range from 0 to 2,147,483,647).</param>
  /// 
  /// <returns>
  /// A string of binaries
  /// </returns>
  internal static string ToBinary(uint integer)
  {
    // Return data
    string binaries;


    // Convert integer to binary.
    binaries = Convert.ToString(integer, 2);

    // Finally.
    return binaries;
  }


  /// <summary>
  /// Converts positive integers into binary.
  /// </summary>
  /// 
  /// <param name="integer">A positive integers (range from 0 to 18,446,744,073,709,551,615).</param>
  /// 
  /// <returns>
  /// A string of binaries
  /// </returns>
  internal static string ToBinary(ulong integer)
  {
    // Return data
    string binaries;


    /*
      Covert integer to binary.
      Since Convert.ToString doesn't have a method signature for UInt64,
      we have to cast it to Int64 (long).
    */
    binaries = Convert.ToString((long)integer, 2);

    // Finally.
    return binaries;
  }


  /// <summary>
  /// Converts a positive integers that is greater than max value of ulong type
  /// or 2^64.
  /// </summary>
  /// 
  /// <remarks>
  /// Throws an exception if argument is not a positive bigint.
  /// </remarks>
  /// 
  /// <param name="integer">
  /// A positive bigint integers that is greater than max value of ulong type.
  /// </param>
  /// 
  /// <returns>
  /// A string of binaries
  /// </returns>
  /// 
  /// <exception cref="ArgumentException">
  /// Throws an exception if the input bigint is not positive.
  /// </exception>  
  internal static string ToBinary(BigInteger integer)
  {
    // Return data
    string binaries;

    // Input integer must be positive.
    if (integer < 0) throw new ArgumentException("From ToBinary: bigint Integer must be positive.");

    /*
      Covert integer to binary.
      Since c# doesn't have a built-in method to convert BigInteger to
      binary we have to create our own method to do that and created 
      ConvertIntegerToBinary method.
    */
    binaries = ConvertIntegerToBinary(integer);

    // Finally.
    return binaries;
  }


  /// <summary>
  /// This method converts positive integers to binaries. Can also convert 
  /// integers greater than (2 raised to 64).
  /// </summary>
  /// 
  /// <param name="integer">A positive integers</param>
  /// 
  /// <returns>A string of binaries.</returns>
  private static string ConvertIntegerToBinary(BigInteger integer)
  {
    /*
      Had to create this method because c# doesn't have a built-in 
      methods to convert bigint numbers to binary. 
      The logic used to get the binary in this method is the tradional
      method of converting decimal to binary by subtraction.
    */

    BigInteger decimalForm = integer;
    BigInteger exponent = BigInteger.Log2(decimalForm);

    BigInteger currentPlaceValue;
    const sbyte numbericBase = 2;

    List<char> bits = [];
    string binary;
    

    for (BigInteger i = exponent; !(i <= -1); i--)
    {
      currentPlaceValue = BigInteger.Pow(numbericBase, (int)i);
      // Console.WriteLine($"2 raised to {i} - place value: {currentPlaceValue}");

      if (currentPlaceValue <= decimalForm)
      {
        // Append 1
        bits.Add('1');

        decimalForm -= currentPlaceValue;

        continue;
      }

      // Otherwise, append 0;
      bits.Add('0');
    }

    binary = String.Join("", bits);

    // Finally.
    return binary;
  }















}