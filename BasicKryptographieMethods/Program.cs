using System.Numerics;
internal class Program
{
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

    private static void Main(string[] args)
    {
        Console.WriteLine(EulersTotientFunction(111));
        Console.WriteLine(MobiusFunction(111));
        Console.WriteLine(LeastCommonMultiple(10, 6));
    }
}

/*int phi(int n) 
{ 
    int ret = 1; 
    for (int i = 2; i * i <= n; ++i) 
    { 
        int p = 1; 
        while (n % i == 0) 
        { 
            p *= i; 
            n /= i; 
        } 
        if ((p /= i) >= 1) ret *= p * (i - 1); 
    } 
    return --n ? n * ret : ret; 
}*/
