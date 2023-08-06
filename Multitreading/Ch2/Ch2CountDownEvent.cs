namespace dotnet7_research.Multitreading.Ch2;

public class Ch2CountDownEvent
{
    private static readonly CountdownEvent _countdown = new CountdownEvent(2);

    public static void Start()
    {
        Console.WriteLine("Starting two operations");
        var t1 = new Thread(() => PerformOperation("Operation 1 is completed", 4));
        var t2 = new Thread(() => PerformOperation("Operation 2 is completed", 8));

        t1.Start();
        t2.Start();

        _countdown.Wait();
        Console.WriteLine("Both operations completed");
        _countdown.Dispose();
    }

    private static void PerformOperation(string message, int seconds)
    {
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
        Console.WriteLine(message);
        _countdown.Signal();
    }
}