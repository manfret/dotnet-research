namespace dotnet7_research.Multitreading.Ch3;

public static class Ch3DelegateInTP
{
    public static void Start()
    {
        var threadId = 0;

        var t = new Thread(() => Test(out threadId));
        t.Start(); 
        t.Join();

        Console.WriteLine("Thread id: {0}", threadId);
        Console.WriteLine("---------------------------------");

        var t2 = ThreadPool.QueueUserWorkItem(Callback, "a delegate asyncronous call");
    }

    private static void Callback(object obj)
    {
        Console.WriteLine("Starting a callback...");
        Console.WriteLine("State passed a callback: {0}", obj);
        Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
        Console.WriteLine("Is a background thread: {0}", Thread.CurrentThread.IsBackground);
        Console.WriteLine("Thread pool worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
        Thread.Sleep(20);
        Console.WriteLine("End");
    }

    private static string Test(out int threadId)
    {
        Console.WriteLine("Starting...");
        Console.WriteLine("Is thread pool thread: {0}", Thread.CurrentThread.IsThreadPoolThread);
        Console.WriteLine("Is a background thread: {0}", Thread.CurrentThread.IsBackground);
        Thread.Sleep(TimeSpan.FromSeconds(2));
        threadId = Thread.CurrentThread.ManagedThreadId;
        return string.Format("Thread pool worker thread id was: {0}", threadId);
    }
}