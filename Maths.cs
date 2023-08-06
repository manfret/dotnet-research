using System.Numerics;

namespace dotnet7_research;

public class Maths
{
    public static void RunIt()
    {
        var lst1 = new List<int> { 1, 2 };
        var lst2 = new List<int> { 3, 4 };

        var tl = (lst1, lst2);
        Console.WriteLine(tl);


        var a = 2;
        var b = 4;

        object aa = (object)a;
        var t = (aa, b);
        Console.WriteLine(t);
        var c = t;
        t = (a + "2", b);
        Console.WriteLine(c);
        Console.WriteLine(t);

        var res = a.Add(b);
        Console.WriteLine($"a + b = {res}");

        MathExtensions.PrintPeriod<int>();
    }
}

public static class MathExtensions
{
    public static T Add<T>(this T left, T right) where T : INumber<T>
    {
        return left + right;
    }

    public static void PrintPeriod<T>() where T : IMinMaxValue<T>, INumber<T>, INumberBase<T>
    {
        var before = T.CreateChecked(2);
        for (var i = T.MinValue; i <= T.MinValue.Add(before); i++)
        {
            Console.WriteLine(i);
        }
    }
}