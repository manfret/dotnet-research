namespace dotnet7_research.Multitreading.Ch5;

public static class Ch5AsyncVoid
{
    public static void Start()
    {
        /*
        var t = AsyncTask();
        t.Wait();

        Console.WriteLine();

        AsyncVoid();
        Thread.Sleep(TimeSpan.FromSeconds(3));

        Console.WriteLine();

        t = AsyncTaskWithErrors();
        while (!t.IsFaulted)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        Console.WriteLine(t.Exception);

        Console.WriteLine();

        //try
        //{
        //    AsyncVoidWithErrors();
        //    Thread.Sleep(TimeSpan.FromSeconds(3));
        //}
        //catch (Exception e)
        //{
        //    Console.WriteLine(e);
        //}

        Console.WriteLine();

        int[] numbers = new[] { 1, 2, 3, 4, 5 };
        Array.ForEach(numbers, async number =>
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            if (number == 3) throw new Exception("Boom!");
            Console.WriteLine(number);
        });

        
        */

        var t1 = AsyncTask();
        var t2 = AsyncTask();

        Console.ReadLine();
    }

    private static async Task AsyncTaskWithErrors()
    {
        string result = await GetInfoAsync("AsyncTaskExceptions", 2);
        Console.WriteLine(result);
    }

    private static async void AsyncVoidWithErrors()
    {
        string result = await GetInfoAsync("AsyncTaskExceptions", 2);
        Console.WriteLine(result);
    }

    private static async Task AsyncTask()
    {
        string result = await GetInfoAsync("AsyncTask", 2);
        Console.WriteLine(result);
    }

    private static async Task AsyncVoid()
    {
        string result = await GetInfoAsync("AsyncVoid", 2);
        Console.WriteLine(result);
    }

    private static async Task<string> GetInfoAsync(string name, int seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
        if (name.Contains("Exception")) throw new Exception($"Boom from {name}!");
        return $"Task {name} is running on a thread id {Environment.CurrentManagedThreadId}. " +
               $"Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}";
    }
}