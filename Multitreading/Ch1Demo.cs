
namespace dotnet7_research.Multitreading;

public class Ch1Demo
{
    public static void Start()
    {
        var t = new Thread(FaultyThread);
        t.Start();
        t.Join();

        try
        {
            t = new Thread(BadFaultyThread);
            t.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine("Never here...");
        }
    }

    private static void BadFaultyThread()
    {
        Console.WriteLine("Starting a faulty thread...");
        Thread.Sleep(TimeSpan.FromSeconds(2));
        throw new Exception("Boom!");
    }

    private static void FaultyThread()
    {
        try
        {
            Console.WriteLine("Starting a faulty thread...");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            throw new Exception("Boom!");
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception handled -> {0}", e.Message);
        }
    }
}