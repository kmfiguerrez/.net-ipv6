const string ipv6Add = "aaaa:b:c:d:e::f:0:1:5";

IPv6.IsValidIPv6(ipv6Add);

// string sub = ipv6Add.Substring(ipv6Add.Length -2, 2);
string sub = ipv6Add[(ipv6Add.Length - 2)..];

// Console.WriteLine(sub);