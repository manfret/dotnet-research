namespace dotnet7_research.Multitreading.Ch3;

public static class Ch3Timer
{
    private static Timer? _timer;

    public static void Start()
    {
        Console.WriteLine("Press 'Enter' to stop the timer...");
        var start = DateTime.UtcNow;
        _timer = new Timer(_ => TimerOperation(start), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
        Thread.Sleep(TimeSpan.FromSeconds(6));
        _timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(4));
        Console.ReadLine();
        _timer.Dispose();
    }

    private static void TimerOperation(DateTime start)
    {
        var elapsed = DateTime.UtcNow - start;
        Console.WriteLine("{0} seconds from {1}. Times thread pool thread id: {2}", elapsed.Seconds, start,
            Environment.CurrentManagedThreadId);

    }
}