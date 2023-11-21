using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

internal class Program
{
    public static char[] characters = new char[] {'#', 'a', 'b', 'c', 'd', 'e', 
        'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 
        'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', ' ', '1', '2', 
        '3', '4', '5', '6', '7', '8', '9', '0'};
    public static bool IsPrime(BigInteger n)
    {
        if (n == 2)
            return true;

        if (n % 2 == 0)
            return false;

        for (BigInteger k = 3; k * k <= n; k += 2)
        {
            if (n % k == 0)
                return false;
        }
        
        return true;
    }
    public static BigInteger Sqrt(BigInteger a)
    {
        if (a < 0)
            return 0;
        if (a < 4)
            return a == 0 ? 0 : 1;

        var k = 2 * Sqrt((a - a % 4) / 4);
        return a < (k + 1)*(k + 1) ? k : k + 1;
    }


    private static BigInteger F(BigInteger x, BigInteger n)
    {
        return (x * x + 1) % n;
    }
    public static BigInteger PowMod(BigInteger a, BigInteger b, BigInteger m)
    {
        if (b == 0)
            return 1;
        if (b == 1 || a == 0)
            return a % m;
        BigInteger res = 1;
        while (b > 0)
        {
            if (b % 2 == 1)
                res = (res * a) % m;
            a = (a * a) % m;
            b /= 2;
        }

        return res; 
    }
    //1
    private static BigInteger EulersTotientFunction(BigInteger n)
    {
        BigInteger ret = 1;

        for (BigInteger i = 2; i * i <= n; i++)
        {
            BigInteger p = 1;
            while (n % i == 0)
            {
                p *= i;
                n /= i;
            }
            if ((p /= i) >= 1) ret *= p * (i - 1);
        }
        return --n > 2 ? n * ret : ret;
    }


    private static BigInteger MobiusFunction(BigInteger n)
    {
        if (n == 1) return 1;
        BigInteger count = 1;
        for (BigInteger i = 2; i * i <= n; i++)
        {
            if (n % i == 0)
            {
                count++;
                n /= i;
                if (n % i == 0) return 0;
            }
        }
        return (count % 2 == 0) ? 1 : -1;
    }



    public static BigInteger LeastCommonMultiple(BigInteger a, BigInteger b)
    {
        return (a * b) / GreatestCommonDivisor(a, b);
    }

    public static BigInteger GreatestCommonDivisor(BigInteger a, BigInteger b)
    {
        while (b != 0)
        {
            BigInteger temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        for (BigInteger x = 1; x < m; x++)
        {
            if ((a * x) % m == 1)
                return x;
        }
        return -1;
    }

    static BigInteger ChineseRemainder(BigInteger[] a, BigInteger[] m)
    {
        int n = a.Length;

        BigInteger M = 1;
        for (int i = 0; i < n; i++)
        {
            M *= m[i];
        }

        BigInteger result = 0;
        for (int i = 0; i < n; i++)
        {
            BigInteger Mi = M / m[i];
            BigInteger Ni = ModInverse(Mi, m[i]);
            result += a[i] * Ni * Mi;
        }
        result = result % M;
        if (result < 0)
            result += M;

        return result;
    }

    public static int? Legendre(BigInteger a, BigInteger p)
    {
        if (p < 3 || !IsPrime(p))
            return null;

        if (a % p == 0)
            return 0;

        return PowMod(a, (p - 1) / 2, p) == 1 ? 1 : -1;
    }

    public static BigInteger Jacobi(BigInteger a, BigInteger b)
    {
        if (GreatestCommonDivisor(a, b) != 1)
            return 0;

        a %= b;
        BigInteger t = 1;
        while (a != 0)
        {
            while (a % 2 == 0)
            {
                a /= 2;
                var r = b % 8;
                if (r == 3 || r == 5)
                    t = -t;
            }
            var temp = a;
            a = b;
            b = a;

            if (a % 4 == 3 && b % 4 == 3)
                t = -t;
            a %= b;
        }
        return b == 1 ? t : 0;
    }

    public static (BigInteger, BigInteger) FactorizePollard(BigInteger n)
    {
        var rand = new Random();
        BigInteger x = n < int.MaxValue ? rand.Next(0, (int)n) : rand.Next(0, int.MaxValue);
        BigInteger y = x;
        BigInteger d = 1;

        while (d == 1)
        {
            x = F(x, n);
            y = F(F(y, n), n);
            d = GreatestCommonDivisor(n, BigInteger.Abs(x - y));
        }

        return d == n ? (-1, -1) : (d, n / d);
    }

    private static bool IsPrimeMillerRabin(BigInteger n, int k)
    {
        if (n == 2 || n == 3) return true;
        if (n < 2 || n % 2 == 0) return false;
        var d = n - 1;
        var r = new BigInteger(0);
        var rand = new Random();
        while(d % 2 == 0)
        {
            d = d / 2;
            r++;
        }
        for(int i = 0; i < k; i++)
        {
            var a = n < int.MaxValue ? rand.Next(0, (int)n) : rand.Next(0, int.MaxValue);
            var x = PowMod(a, d, n);
            if (a != 1)
            {
                var j = new BigInteger(1);
                while(a != n-1 && j < r)
                {
                    x = PowMod(x, 2, n);
                    j++;
                }
                if (a != n - 1) return false;
            }
        }
        return true;
    }

    public static BigInteger LogBabyStepGiantStep(BigInteger a, BigInteger b, BigInteger n)
    {
        a %= n;
        b %= n;
        var m = Sqrt(n) + 1;
        var g0 = PowMod(a, m, n);
        var g = g0;
        var t = new Dictionary<string, BigInteger>();

        for (var i = new BigInteger(1); i <= m; i++)
        {
            t.Add(g.ToString(), i);
            g = (g * g0) % n;
        }

        for (var j = new BigInteger(0); j < m; j++)
        {
            var y = (b * PowMod(a, j, n)) % n;
            if (t.ContainsKey(y.ToString()))
            {
                return m * t[y.ToString()] - j;
            }
        }

        return -1;
    }

    public static (BigInteger, BigInteger) SqrtCipolla(BigInteger n, BigInteger p)
    {
        if (Legendre(n, p) != 1)
        {
            return (-1, 0);
        }

        BigInteger a = 0;
        BigInteger w2;
        while (true)
        {
            w2 = (a * a + p - n) % p;
            if (Legendre(w2, p) != 1)
                break;
            a++;
        }

        var finalW = w2;
        (BigInteger, BigInteger) MulExtended((BigInteger, BigInteger) aa, (BigInteger, BigInteger) bb)
        {
            return ((aa.Item1 * bb.Item1 + aa.Item2 * bb.Item2 * finalW) % p,
                    (aa.Item1 * bb.Item2 + bb.Item1 * aa.Item2) % p);
        }

        var r = (new BigInteger(1), new BigInteger(0));
        var s = (a, new BigInteger(1));
        var nn = (p + 1) / 2;
        while (nn > 0)
        {
            if (nn % 2 != 0)
            {
                r = MulExtended(r, s);
            }
            s = MulExtended(s, s);
            nn /= 2;
        }

        if (r.Item2 != 0 || r.Item1 * r.Item1 % p != n)
        {
            return (0, -1);
        }

        return (r.Item1, p - r.Item1);
    }

    private static (BigInteger,BigInteger, BigInteger) RSA(BigInteger p, BigInteger q)
    {
        if (IsPrime(p) && IsPrime(q))
        {
            var n = p * q;
            var phiN = EulersTotientFunction(n);
            var a = CalculateAForRSA(phiN);
            var e = CalculateEForRSA(a, phiN);
            return (e, a, n);
        }
        else
        {
            Console.WriteLine($"{p} or {q} is not prime number");
            return (0, 0, 0);
        }
    }

    public static BigInteger CalculateEForRSA(BigInteger a, BigInteger n)
    {
        
        var rand = new Random();
        BigInteger e = n < int.MaxValue ? rand.Next(0, (int)n) : rand.Next(0, int.MaxValue); ;

        while ((e*a) % n != 1)
        {
            e = n < int.MaxValue ? rand.Next(0, (int)n) : rand.Next(0, int.MaxValue);
        }

        return e;   
    }

    public static BigInteger CalculateAForRSA(BigInteger n)
    {
        var rand = new Random();
        BigInteger a = n < int.MaxValue ? rand.Next(0, (int)n) : rand.Next(0, int.MaxValue); 
        while (GreatestCommonDivisor(a, n) != 1)
        {
           a = n < int.MaxValue ? rand.Next(0, (int)n) : rand.Next(0, int.MaxValue);
        } 
        return a;
    }

    private static List<BigInteger> EncodeRSA(string s, BigInteger e, BigInteger n)
    {
        var result = new List<BigInteger>();
        foreach(var c in s)
        {
            var index = Array.IndexOf(characters, c);
            result.Add(PowMod(index, e, n));
        }
        return result;
    }

    private static string DecodeRSA(List<BigInteger> code, BigInteger a, BigInteger n)
    {
        var builder = new StringBuilder();
        foreach(var num in code)
        {
            int index = (int)PowMod(num, a, n);
            builder.Append(characters[index]);
        }
        return builder.ToString();
    }

    private static void Main(string[] args)
    {
        //Console.WriteLine(LogBabyStepGiantStep(5, 3, 23));//5^x≡3(mod23)
        var keys = RSA(101, 103);
        Console.WriteLine($"Public key: {keys.Item1} {keys.Item3}");
        Console.WriteLine($"Private key: {keys.Item2} {keys.Item3}");
        string s = "hello world";
        var codedS = EncodeRSA(s, keys.Item1, keys.Item3);
        foreach(var code in codedS)
        {
            Console.Write(code + " ");
        }
        Console.WriteLine();
        string decodedS = DecodeRSA(codedS, keys.Item2, keys.Item3);
        Console.WriteLine(decodedS);
    }
}

