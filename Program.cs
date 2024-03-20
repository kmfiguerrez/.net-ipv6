const string ipv6Add = "0000:0000:3:0000:0000:0000:0:0008";

// Console.WriteLine(IPv6.IsValidIPv6(ipv6Add));



IPv6ReturnData expanded = IPv6.Expand(ipv6Add);
if (!expanded.success)
{
    Console.WriteLine(expanded.message);
}
else
{
Console.WriteLine(expanded.data);
}


IPv6ReturnData abbreviated = IPv6.Abbreviate(expanded.data);

if (!abbreviated.success)
{
    Console.WriteLine(abbreviated.message);
}
else
{
Console.WriteLine(abbreviated.data);
}




// Console.WriteLine(str);