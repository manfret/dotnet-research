namespace dotnet7_research.Multitreading.Ch2;

public class Ch2AutoResetEvent
{
    private static readonly AutoResetEvent _workerEvent = new(false);
    private static readonly AutoResetEvent _mainEvent = new(false);

    public static void Start()
    {
        var t = new Thread(() => Process(10));
        t.Start();

        Console.WriteLine("Waiting for another thread to complete work...");
        _workerEvent.WaitOne();
        Console.WriteLine("First operation is completed!");
        Console.WriteLine("Performing an operation on a main thread at {0}...", DateTime.UtcNow.ToLongTimeString());
        Thread.Sleep(TimeSpan.FromSeconds(5));
        _mainEvent.Set();
        Console.WriteLine("Rinnung the second operation on a second thread at {0}...", DateTime.UtcNow.ToLongTimeString());
        _workerEvent.WaitOne();
        Console.WriteLine("Second operation is completed at {0}!", DateTime.UtcNow.ToLongTimeString());
    }

    private static void Process(int seconds)
    {
        Console.WriteLine("Starting a long running work at {0}...", DateTime.UtcNow.ToLongTimeString());
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
        Console.WriteLine("Work is done at {0}!", DateTime.UtcNow.ToLongTimeString());
        _workerEvent.Set();
        Console.WriteLine("Waiting for a main thread to complete its work");
        _mainEvent.WaitOne();
        Console.WriteLine("Starting second operations at {0}...", DateTime.UtcNow.ToLongTimeString());
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
        Console.WriteLine("Work is done at {0}!", DateTime.UtcNow.ToLongTimeString());
        _workerEvent.Set();
    }

    private static void Do(int i)
    {
        _workerEvent.WaitOne();
        Console.WriteLine("Before {0}", i);
        Thread.Sleep(TimeSpan.FromSeconds(10));
        Console.WriteLine("After {0}", i);
        _workerEvent.Set();
    }
}