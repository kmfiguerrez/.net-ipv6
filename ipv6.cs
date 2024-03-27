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
  /// Throws an exception if provided with invalid IPv6 address.
  /// </remarks>
  /// 
  /// <param name="ipv6Address">A string IPv6 address.</param>
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
  /// Throws an exception if provided with invalid IPv6 address.
  /// </remarks>
  /// 
  /// <param name="ipv6Address">A string IPv6 address.</param>
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


    
    // Try to expand the address first
    try
    {
      /*
        Input data validation of this method relies on Expand method because
        it also validates the same argument which is the IPv6 addresss of
        type string. This way avoids slowing down the application by not
        having the same data validation.
      */
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
  /// Converts hexadecimals to binary.
  /// </summary>
  /// 
  /// <remarks>
  /// Notes:
  ///   <list type="bullet">
  ///     <item>
  ///       <description>
  ///         Argument <c><paramref name="hexadecimals"/></c> is typed <c>string</c>, not a number literal prefixed
  ///         with <c>0x</c> or <c>0X</c> and must be positive.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         This method throws an exception if not provided with the 
  ///         desired <c>hexadecimals</c> argument.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         Argument <c><paramref name="includeLeadingZeroes"/></c> when
  ///         set to <c>true</c> will include leading zeroes of the returned
  ///         binaries.
  ///       </description>
  ///     </item>
  ///   </list>
  /// </remarks>
  /// 
  /// <param name="hexadecimals">A string positive hex digits.</param>
  /// <param name="includeLeadingZeroes">Boolean.</param>
  /// 
  /// <returns>
  /// A string of binaries
  /// </returns>
  /// 
  /// <exception cref="ArgumentException">
  /// Throws an exception if the input hexadecimals is not valid.
  /// </exception>  
  internal static string ToBinary(string hexadecimals, bool includeLeadingZeroes = false)
  {
    // Sanitize input data first.
    hexadecimals = hexadecimals.Trim().ToLower();

    // Leading zeros pattern.
    const string leadingZeroPattern = @"^0+";

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

      /*
        Because toString method does not add leading zeros
        we have to prepend leading zeros which is important
        at this part of the process.      
      */
      binaries += binary.PadLeft(4, '0');
    }

    /*
      The final binaries leading zeroes does not affect the value.
      By default are omitted. Otherwise included if param includeLeadingZeroes
      set to true.
    */
    if (!includeLeadingZeroes)
    {
      binaries = Regex.Replace(binaries, leadingZeroPattern, "");
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
  /// Converts a positive integers that is greater than 2^64 to binary.
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
  /// <remarks>
  /// If <c>skipArgumentValidation</c> is set to <c>true</c>, 
  /// this method performs argument validation and throws an exception.
  /// </remarks>
  /// 
  /// <param name="integer">A positive integers</param>
  /// 
  /// <returns>A string of binaries.</returns>
  /// 
  /// <exception cref="ArgumentException"></exception>   
  private static string ConvertIntegerToBinary(BigInteger integer, bool skipArgumentValidation = true)
  {
    /*
      Had to create this method because c# doesn't have a built-in 
      methods to convert bigint numbers to binary. 
      The logic used to get the binary in this method is the tradional
      method of converting decimal to binary by subtraction.
      Input data validation for this method relies on the method caller
      of this method to avoid slowing down the application by having
      multiple data validation.      
    */

    BigInteger decimalForm = integer;
    BigInteger exponent = BigInteger.Log2(decimalForm);

    BigInteger currentPlaceValue;
    const sbyte numbericBase = 2;

    List<char> bits = [];
    string binary;
    

    /*
      Validate input data. Disabled by default. 
      It's up to the method caller.
    */
    if (skipArgumentValidation == false)
    {
      if (integer < 0) throw new ArgumentException("From ConvertIntegerToBinary: BigInteger must be positive.");
    }     

    // Get the binaries.
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


  /// <summary>
  /// Converts binaries into hexadecimals.
  /// </summary>
  /// 
  /// <remarks>
  /// Notes:
  ///   <list type="bullet">
  ///     <item>
  ///       <description>
  ///         Argument <c><paramref name="binary"/></c> is typed <c>string</c>, not a number literal prefixed
  ///         with <c>0b</c> or <c>0B</c> and must be positive.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         This method throws an exception if not provided with the 
  ///         desired argument.
  ///       </description>
  ///     </item>
  ///   </list>
  /// </remarks>
  /// 
  /// <param name="binary">A string positive binaries.</param>
  /// 
  /// <returns>A string of hexadecimals.</returns>
  /// 
  /// <exception cref="ArgumentException">
  /// Throws an exception if given an invalid binaries.
  /// </exception>
  internal static string ToHex(string binary)
  {
    // Sanitize input data first.
    binary = binary.Trim();

    // Return data.
    string hexadecimals;


    // Validate input data first.
    if (!IsHex(binary)) throw new ArgumentException("From ToHex: Invalid argument binaries provided.");

    /*
      Because c# doesn't have a built-in method for converting integers
      greater than 2^64 or integers of type BigInteger, we have to create
      a method that converts typed string binaries to hexadecimals.
    */
    hexadecimals = ConvertBinaryToHex(binary);
    
    // Finally.
    return hexadecimals;
  }


  /// <summary>
  /// Converts positive integers into hexadecimals.
  /// </summary>
  /// 
  /// <param name="integer">Positive integers range from 0 to 255.</param>
  /// 
  /// <returns>A string of hexadecimals.</returns>
  internal static string ToHex(byte integer)
  {
    // Return data.
    string hexadecimals;


    // Convert integer to hexadecimals.
    hexadecimals = Convert.ToString(integer, 16);

    // Finally
    return hexadecimals;
  }



  /// <summary>
  /// Converts positive integers into hexadecimals.
  /// </summary>
  /// 
  /// <param name="integer">Positive integers range from 0 to 65,535.</param>
  /// 
  /// <returns>A string of hexadecimals.</returns>
  internal static string ToHex(ushort integer)
  {
    // Return data.
    string hexadecimals;


    // Convert integer to hexadecimals.
    hexadecimals = Convert.ToString(integer, 16);

    // Finally
    return hexadecimals;
  }


  /// <summary>
  /// Converts positive integers into hexadecimals.
  /// </summary>
  /// 
  /// <param name="integer">Positive integers range from 0 to 4,294,967,295.</param>
  /// 
  /// <returns>A string of hexadecimals.</returns>
  internal static string ToHex(uint integer)
  {
    // Return data.
    string hexadecimals;


    // Convert integer to hexadecimals.
    hexadecimals = Convert.ToString(integer, 16);

    // Finally
    return hexadecimals;
  }


  /// <summary>
  /// Converts positive integers into hexadecimals.
  /// </summary>
  /// 
  /// <param name="integer">Positive integers range from 0 to 18,446,744,073,709,551,615.</param>
  /// 
  /// <returns>A string of hexadecimals.</returns>
  internal static string ToHex(ulong integer)
  {
    // Return data.
    string hexadecimals;


    /*
      Covert integer to Hexadecimals.
      Since Convert.ToString doesn't have a method signature for UInt64,
      we have to cast it to Int64 (long).
    */
    hexadecimals = Convert.ToString((long)integer, 16);

    // Finally
    return hexadecimals;
  }    


  /// <summary>
  /// Converts a positive integers that is greater than 2^64 to hexadecimals.
  /// </summary>
  /// 
  /// <remarks>
  /// Throws an exception if argument is not a positive bigint.
  /// </remarks>
  /// 
  /// <param name="integer">
  /// A positive bigint integers that is greater than 2^64.
  /// </param>
  /// 
  /// <returns>
  /// A string of hexadecimals
  /// </returns>
  /// 
  /// <exception cref="ArgumentException">
  /// Throws an exception if the input bigint is not positive.
  /// </exception>  
  internal static string ToHex(BigInteger integer)
  {
    // Return data
    string hexadecimals;

    // Input integer must be positive.
    if (integer < 0) throw new ArgumentException("From ToHex: bigint Integer must be positive.");

    /*
      Covert integer to hexadecimals.
      Since c# doesn't have a built-in method to convert BigInteger to
      hexadecimals we have to use the ConvertIntegerToBinary method.
    */
    string binaries = ConvertIntegerToBinary(integer);

    // Then from binary to hexadecimals.
    hexadecimals = ConvertBinaryToHex(binaries);

    // Finally.
    return hexadecimals;
  }


  /// <summary>
  /// Converts binary to hexadecimals.
  /// </summary>
  /// 
  /// <remarks>
  /// If <c>skipArgumentValidation</c> is set to <c>true</c>, 
  /// this method performs argument validation and throws an exception.
  /// </remarks>
  /// 
  /// <param name="binary">A string of postive binaries.</param>
  /// 
  /// <returns>A string of positive hex digits.</returns>
  /// 
  /// <exception cref="ArgumentException"></exception>  
  private static string ConvertBinaryToHex(string binary, bool skipArgumentValidation = true)
  {
    /*
      This method is created because c# doesn't have a built-in method
      for converting integers greater than 2^64.
      Input data validation for this method relies on the method caller
      of this method to avoid slowing down the application by having
      multiple data validation.
    */

    // Sanitize input data first.
    binary = binary.Trim();

    bool divisibleByFour = binary.Length % 4 == 0;
    
    List<string> nibbles = [];
    string hexadecimals;

    
    /*
      Validate input data. Disabled by default. 
      It's up to the method caller.
    */
    if (skipArgumentValidation == false)
    {
      if (!IsBinary(binary)) throw new ArgumentException("From ConvertBinaryToHex: Invalid binaries provided.");
    }  

    // Check if not divisible by four.
    if (!divisibleByFour)
    {
      // Extract the first x bits.
      sbyte bits = (sbyte)(binary.Length % 4);
      // Turn it to hex and append it to list of nibbles.
      byte integer =  Convert.ToByte(binary.Substring(0, bits), 2);
      string hex = Convert.ToString(integer, 16);
      nibbles.Add(hex);

      // Then remove the extracted bits to binary.
      binary = binary.Substring(bits);
    }

    // Otherwise divisible by four.
    for (int i = 0; i < binary.Length; i+=4)
    {
      // Extract four bits on each iteration.
      string nibble = binary.Substring(i,4);
      // Convert four bits to decimal form.
      byte integer = Convert.ToByte(nibble, 2);
      // From decimal to hexadecimals.
      string hex = Convert.ToString(integer, 16);
      // Console.WriteLine($"Bits: {nibble} - Dec: {integer} - Hex: {hex}");

      // Append it to the list.
      nibbles.Add(hex);
    }

    // Turn list of nibbles into a single string.
    hexadecimals = String.Join("", nibbles);
    // Finally.
    return hexadecimals;
  }



  /// <summary>
  /// Converts a string of binaries or hexadecimals into decimal form (integer).
  /// </summary>
  /// 
  /// <remarks>
  /// Notes:
  ///   <list type="bullet">
  ///     <item>
  ///       <description>
  ///         The <c>binOrHex</c> argument is either a string of binaries or
  ///         hexadecimals. Meaning it should not be prefixed with <c>0b</c> or <c>0x</c>.
  ///         The integral value should also be in range based on the 
  ///         return data type.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         The <c>fromBase</c> argument accepts only two integer values: 
  ///         <c>2</c> for binaries and <c>16</c> for hexadecimals. 
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         This method throws an exception if not provided with the 
  ///         desired arguments.
  ///       </description>
  ///     </item>
  ///   </list>
  /// </remarks>
  /// 
  /// <param name="binORHex">A string of binaries or hexadecimals.</param>
  /// <param name="fromBase">An integer with only two possible values 2 and 16.</param>
  /// 
  /// <returns>Typed byte integers.</returns>
  /// 
  /// <exception cref="ArgumentException"></exception>
  internal static byte ToByteDecimal(string binORHex, sbyte fromBase)
  {
    /*
      Note!
      The toByte method throws multiple exceptions. In this code
      it could throw an OverflowException if received values that either
      too large or too small.
      Make sure to filter input binOrHex values in the method caller of
      this method.
    */

    // Argument cannot be null or empty.
    if (binORHex == null || binORHex == "") throw new ArgumentException("From ToByteDecimal: Did not provide argument.");

    // Return data.
    byte decimals;


    // Determine what base number system to work on.
    switch (fromBase)
    {
      case 2: {
        // Sanitize user input first.
        string binaries = binORHex.Trim();

        // Validate input data.
        if (!IsBinary(binaries)) throw new ArgumentException("From ToByteDecimal: Invalid binaries provided.");

        // Convert binaries to decimal form.
        decimals = Convert.ToByte(binaries, 2);

        return decimals;
      }
      case 16: {
        // Sanitize user input first.
        string hexadecimals = binORHex.Trim();

        // Validate input data.
        if (!IsHex(hexadecimals)) throw new ArgumentException("From ToByteDecimal: Invalid hexadecimals provided.");

        // Convert binaries to decimal form.
        decimals = Convert.ToByte(hexadecimals, 16);

        return decimals;
      }           
      default: {
        throw new ArgumentException("From ToByteDecimal: Invalid base number system provided.");
      }
    }
  }


  /// <summary>
  /// Converts a string of binaries or hexadecimals into decimal form (integer).
  /// </summary>
  /// 
  /// <remarks>
  /// Notes:
  ///   <list type="bullet">
  ///     <item>
  ///       <description>
  ///         The <c>binOrHex</c> argument is either a string of binaries or
  ///         hexadecimals. Meaning it should not be prefixed with <c>0b</c> or <c>0x</c>.
  ///         The integral value should also be in range based on the 
  ///         return data type.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         The <c>fromBase</c> argument accepts only two integer values: 
  ///         <c>2</c> for binaries and <c>16</c> for hexadecimals. 
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         This method throws an exception if not provided with the 
  ///         desired arguments.
  ///       </description>
  ///     </item>
  ///   </list>
  /// </remarks>
  /// 
  /// <param name="binORHex">A string of binaries or hexadecimals.</param>
  /// <param name="fromBase">An integer with only two possible values 2 and 16.</param>
  /// 
  /// <returns>Typed ushort integers.</returns>
  /// 
  /// <exception cref="ArgumentException"></exception>
  internal static ushort ToUshortDecimal(string binORHex, sbyte fromBase)
  {
    /*
      Note!
      The ToUInt16 method throws multiple exceptions. In this code
      it could throw an OverflowException if received values that either
      too large or too small.
      Make sure to filter input binOrHex values in the method caller of
      this method.
    */

    // Argument cannot be null or empty.
    if (binORHex == null || binORHex == "") throw new ArgumentException("From ToUshortDecimal: Did not provide argument.");

    // Return data.
    ushort decimals;


    // Determine what base number system to work on.
    switch (fromBase)
    {
      case 2: {
        // Sanitize user input first.
        string binaries = binORHex.Trim();

        // Validate input data.
        if (!IsBinary(binaries)) throw new ArgumentException("From ToUshortDecimal: Invalid binaries provided.");

        // Convert binaries to decimal form.
        decimals = Convert.ToUInt16(binaries, 2);

        return decimals;
      }
      case 16: {
        // Sanitize user input first.
        string hexadecimals = binORHex.Trim();

        // Validate input data.
        if (!IsHex(hexadecimals)) throw new ArgumentException("From ToUshortDecimal: Invalid hexadecimals provided.");

        // Convert binaries to decimal form.
        decimals = Convert.ToUInt16(hexadecimals, 16);

        return decimals;
      }           
      default: {
        throw new ArgumentException("From ToUshortDecimal: Invalid base number system provided.");
      }
    }
  }  


  /// <summary>
  /// Converts a string of binaries or hexadecimals into decimal form (integer).
  /// </summary>
  /// 
  /// <remarks>
  /// Notes:
  ///   <list type="bullet">
  ///     <item>
  ///       <description>
  ///         The <c>binOrHex</c> argument is either a string of binaries or
  ///         hexadecimals. Meaning it should not be prefixed with <c>0b</c> or <c>0x</c>.
  ///         The integral value should also be in range based on the 
  ///         return data type.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         The <c>fromBase</c> argument accepts only two integer values: 
  ///         <c>2</c> for binaries and <c>16</c> for hexadecimals. 
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         This method throws an exception if not provided with the 
  ///         desired arguments.
  ///       </description>
  ///     </item>
  ///   </list>
  /// </remarks>
  /// 
  /// <param name="binORHex">A string of binaries or hexadecimals.</param>
  /// <param name="fromBase">An integer with only two possible values 2 and 16.</param>
  /// 
  /// <returns>Typed uint integers.</returns>
  /// 
  /// <exception cref="ArgumentException"></exception>
  internal static uint ToUintDecimal(string binORHex, sbyte fromBase)
  {
    /*
      Note!
      The ToUInt32 method throws multiple exceptions. In this code
      it could throw an OverflowException if received values that either
      too large or too small.
      Make sure to filter input binOrHex values in the method caller of
      this method.
    */

    // Argument cannot be null or empty.
    if (binORHex == null || binORHex == "") throw new ArgumentException("From ToUintDecimal: Did not provide argument.");

    // Return data.
    uint decimals;


    // Determine what base number system to work on.
    switch (fromBase)
    {
      case 2: {
        // Sanitize user input first.
        string binaries = binORHex.Trim();

        // Validate input data.
        if (!IsBinary(binaries)) throw new ArgumentException("From ToUintDecimal: Invalid binaries provided.");

        // Convert binaries to decimal form.
        decimals = Convert.ToUInt32(binaries, 2);

        return decimals;
      }
      case 16: {
        // Sanitize user input first.
        string hexadecimals = binORHex.Trim();

        // Validate input data.
        if (!IsHex(hexadecimals)) throw new ArgumentException("From ToUintDecimal: Invalid hexadecimals provided.");

        // Convert binaries to decimal form.
        decimals = Convert.ToUInt32(hexadecimals, 16);

        return decimals;
      }           
      default: {
        throw new ArgumentException("From ToUintDecimal: Invalid base number system provided.");
      }
    }
  }  


  /// <summary>
  /// Converts a string of binaries or hexadecimals into decimal form (integer).
  /// </summary>
  /// 
  /// <remarks>
  /// Notes:
  ///   <list type="bullet">
  ///     <item>
  ///       <description>
  ///         The <c>binOrHex</c> argument is either a string of binaries or
  ///         hexadecimals. Meaning it should not be prefixed with <c>0b</c> or <c>0x</c>.
  ///         The integral value should also be in range based on the 
  ///         return data type.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         The <c>fromBase</c> argument accepts only two integer values: 
  ///         <c>2</c> for binaries and <c>16</c> for hexadecimals. 
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         This method throws an exception if not provided with the 
  ///         desired arguments.
  ///       </description>
  ///     </item>
  ///   </list>
  /// </remarks>
  /// 
  /// <param name="binORHex">A string of binaries or hexadecimals.</param>
  /// <param name="fromBase">An integer with only two possible values 2 and 16.</param>
  /// 
  /// <returns>Typed ulong integers.</returns>
  /// 
  /// <exception cref="ArgumentException"></exception>
  internal static ulong ToUlongDecimal(string binORHex, sbyte fromBase)
  {
    /*
      Note!
      The ToUInt64 method throws multiple exceptions. In this code
      it could throw an OverflowException if received values that either
      too large or too small.
      Make sure to filter input binOrHex values in the method caller of
      this method.
    */

    // Argument cannot be null or empty.
    if (binORHex == null || binORHex == "") throw new ArgumentException("From ToUlongDecimal: Did not provide argument.");

    // Return data.
    ulong decimals;


    // Determine what base number system to work on.
    switch (fromBase)
    {
      case 2: {
        // Sanitize user input first.
        string binaries = binORHex.Trim();

        // Validate input data.
        if (!IsBinary(binaries)) throw new ArgumentException("From ToUlongDecimal: Invalid binaries provided.");

        // Convert binaries to decimal form.
        decimals = Convert.ToUInt64(binaries, 2);

        return decimals;
      }
      case 16: {
        // Sanitize user input first.
        string hexadecimals = binORHex.Trim();

        // Validate input data.
        if (!IsHex(hexadecimals)) throw new ArgumentException("From ToUlongDecimal: Invalid hexadecimals provided.");

        // Convert binaries to decimal form.
        decimals = Convert.ToUInt64(hexadecimals, 16);

        return decimals;
      }           
      default: {
        throw new ArgumentException("From ToUlongDecimal: Invalid base number system provided.");
      }
    }
  }


  /// <summary>
  /// Converts a string of binaries or hexadecimals into decimal form (integer).
  /// </summary>
  /// 
  /// <remarks>
  /// Notes:
  ///   <list type="bullet">
  ///     <item>
  ///       <description>
  ///         The <c>binOrHex</c> argument is either a string of binaries or
  ///         hexadecimals. Meaning it should not be prefixed with <c>0b</c> or <c>0x</c>.
  ///         The integral value should also be in range based on the 
  ///         return data type.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         The <c>fromBase</c> argument accepts only two integer values: 
  ///         <c>2</c> for binaries and <c>16</c> for hexadecimals. 
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <description>
  ///         This method throws an exception if not provided with the 
  ///         desired arguments.
  ///       </description>
  ///     </item>
  ///   </list>
  /// </remarks>
  /// 
  /// <param name="binORHex">A string of binaries or hexadecimals.</param>
  /// <param name="fromBase">An integer with only two possible values 2 and 16.</param>
  /// 
  /// <returns>BigInteger.</returns>
  /// 
  /// <exception cref="ArgumentException"></exception>
  internal static BigInteger ToBigIntDecimal(string binORHex, byte fromBase)
  {
    // Sanitize user input first.
    binORHex = binORHex.Trim();

    // Argument cannot be null or empty.
    if (binORHex == null || binORHex == "") throw new ArgumentException("From ToBigIntDecimal: Did not provide argument.");

    // Return data.
    BigInteger decimals;


    // Determine what base number system to work on.
    switch (fromBase)
    {
      case 2: {
        string binaries = binORHex;

        // Validate input data.
        if (!IsBinary(binaries)) throw new ArgumentException("From ToBigIntDecimal: Invalid binaries provided.");

        /*
          Convert binaries to decimal form.
          Because c# doesn't have a built-in methods to convert binaries greater 
          than 2^64 to integers, had to create ConvertBinaryToDecimal method.
        */
        decimals = ConvertBinaryToDecimal(binaries);

        return decimals;
      }
      case 16: {
        string hexadecimals = binORHex;

        // Validate input data.
        if (!IsHex(hexadecimals)) throw new ArgumentException("From ToBigIntDecimal: Invalid hexadecimals provided.");

        /*
          Convert hexadecimals to decimal form.
          Because c# doesn't have a built-in methods to convert hexadecimals 
          greater than 2^64 to integers, had to used toBinary method and then 
          ConvertBinaryToDecimal method.
        */
        string binaries = ToBinary(hexadecimals);
        decimals = ConvertBinaryToDecimal(binaries);

        return decimals;
      }           
      default: {
        throw new ArgumentException("From ToBigIntDecimal: Invalid base number system provided.");
      }
    }
  }  


  /// <summary>
  /// Converts a string binaries to decimal form (integer).
  /// </summary>
  /// 
  /// <remarks>
  /// If <c>skipArgumentValidation</c> is set to <c>true</c>, 
  /// this method performs argument validation and throws an exception.
  /// </remarks>
  /// 
  /// <param name="binary">A string of binaries</param>
  /// <param name="skipArgumentValidation">Boolean.</param>
  /// 
  /// <returns>Positive BigInteger.</returns>
  /// 
  /// <exception cref="ArgumentException">
  /// Throws an exception if param skipArgumentValidation is true
  /// and if argument binary is invalid.
  /// </exception>
  private static BigInteger ConvertBinaryToDecimal(string binary, bool skipArgumentValidation = true)
  {
    /*
      Had to create this method because c# doesn't have a built-in 
      methods to convert binaries greater than 2^64 to integers. 
    */

    // Sanitize input data first.
    binary = binary.Trim();

    // Bits length.
    ushort bits = (ushort)binary.Length;

    // Return data.
    BigInteger decimals = 0;


    /*
      Validate input data. Disabled by default. 
      It's up to the method caller.
    */
    if (skipArgumentValidation == false)
    {
      if (!IsBinary(binary)) throw new ArgumentException("From ConvertBinaryToDecimal: Invalid binaries provided.");
    }

    // Get the decimal values.
    for (ushort i = bits; i > 0; i--)
    {
      /*
        Subract one from exponent to get the correct place value of the
        current bit position.
      */
      ushort exponent = (ushort)(i - 1);
      BigInteger currentPlaceValue = BigInteger.Pow(2, exponent);

      // Get the current bit from the left.
      ushort currentIndex = (ushort)(bits - i);
      char currentBit = binary[currentIndex];

      // If the current bit is 1 or on, add the place value.
      if (currentBit == '1') 
      {
        decimals += currentPlaceValue;
      }
    }

    // Finally.
    return decimals;
  }













}