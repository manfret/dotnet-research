namespace dotnet7_research;

public class dotnet5
{
    public static void RunIt()
    {
        var c1 = new C1(){P1 = "2"};
    }

    public class C1
    {
        public string P1 { get; init; }

    }
}