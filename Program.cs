using System.Numerics;

string[] ips = new string[128];
BigInteger testBig = BigInteger.Parse("340282366920938463463374607431768211456");
ulong mLong = ulong.MaxValue;
// BigInteger maxLong = BigInteger.Parse(mLong);
BigInteger maxLong = new BigInteger(mLong);



BigInteger numericBase = 20;
sbyte n1 = 5;
sbyte n2 = 2;
BigInteger exponent = BigInteger.Log2(numericBase);
// Console.WriteLine(exponent);
testBig = BigInteger.Pow(2, 128);
Console.WriteLine(testBig);
// Console.WriteLine(BigInteger.Pow(2, 127));



// string res = IPv6.ConvertIntegerToBinary(255);
string res = IPv6.ToBinary(testBig - 1).data;

Console.WriteLine(res);
Console.WriteLine(res.Length);
// Console.WriteLine(IPv6.ToBinary(20).data);











// IPv6ReturnData expanded = IPv6.Expand("0:0:3:4::");
// if (!expanded.success)
// {
//     Console.WriteLine(expanded.data);
// }
// else
// {
// Console.WriteLine(expanded.data);
// }


// IPv6ReturnData abbreviated = IPv6.Abbreviate(expanded.data);

// if (!abbreviated.success)
// {
//     Console.WriteLine(abbreviated.message);
// }
// else
// {
// Console.WriteLine(abbreviated.data);
// }




// Console.WriteLine(str);