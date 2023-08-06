using System.Net;

namespace dotnet7_research;

public class Tuples
{
    public static void RunIt()
    {
        var lst1 = new List<int> { 1, 2 };
        var lst2 = new List<int> { 3, 4 };

        var (l1, l2) = (lst1, lst2);
        Console.WriteLine(string.Join(',', l1));
        Console.WriteLine(string.Join(',', l2));

        l1[0] = 7;

        Console.WriteLine(string.Join(',', lst1));
        Console.WriteLine(string.Join(',', lst2));

        l1 = new List<int> { 9, 0 };

        Console.WriteLine(string.Join(',', lst1));
        Console.WriteLine(string.Join(',', lst2));

        var k = new List<int>();
        switch (k)
        {
            case object o when o.ToString() == "1":
                Console.WriteLine("1");
                break;
        }
    }
}