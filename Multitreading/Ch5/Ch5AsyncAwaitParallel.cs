namespace dotnet7_research.Multitreading.Ch5;

public static class Ch5AsyncAwaitParallel
{
    public static void Start()
    {
        Task t = AsynchronousProcessing();
        t.Wait();
    }

    private static async Task AsynchronousProcessing()
    {
        Task<string> t1 = GetInfoAsync("Task 1", 3);
        Task<string> t2 = GetInfoAsync("Task 2", 5);

        string[] results = await Task.WhenAll(t1, t2);
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
    }

    private static async Task<string> GetInfoAsync(string name, int seconds)
    {
        //таймер - Thread сразу вернется
        //await Task.Delay(TimeSpan.FromSeconds(seconds));
        //Thread.Sleep - блокировака Thread из пула
        await Task.Run(() => Thread.Sleep(TimeSpan.FromSeconds(seconds)));
        return $"Task {name} is running on a thread id {Environment.CurrentManagedThreadId}. " +
               $"Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}";
    }
}