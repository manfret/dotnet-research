namespace dotnet7_research.Multitreading.Ch5;

public static class Ch5TPLVsAsyncAwait
{
    public static void Start()
    {
        Task t = AsyncronyWithTPL();
        t.Wait();

        t = AsyncronyWithAwait();
        t.Wait();
    }

    private static Task AsyncronyWithTPL()
    {
        Task<string> t = GetInfoAsync("Task 1");
        Task t2 = t.ContinueWith(task => Console.WriteLine(task.Result), TaskContinuationOptions.NotOnFaulted);
        Task t3 = t.ContinueWith(task => task.Exception.InnerExceptions, TaskContinuationOptions.OnlyOnFaulted);

        return Task.WhenAny(t2, t3);
    }

    private static async Task AsyncronyWithAwait()
    {
        try
        {
            string result = await GetInfoAsync("Task 2");
            Console.WriteLine(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task<string> GetInfoAsync(string name)
    {
        await Task.Delay(TimeSpan.FromSeconds(2));
        //throw new Exception("Boom!");

        return $"Task {name} is running on a thread id {Environment.CurrentManagedThreadId}. " +
               $"Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}";
    }
}