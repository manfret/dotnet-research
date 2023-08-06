namespace dotnet7_research.Multitreading.Ch4;

public static class Ch4Exceptions
{
    public static void Start()
    {
        Task<int> task;
        try
        {
            //AggregateException
            task = Task.Run(() => TaskMethod("Task 1", 2));
            int result = task.Result;
            Console.WriteLine("Result: {0}", result);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception caught: {0}", e.Message);
        }
        Console.WriteLine("----------------------------------------");
        Console.WriteLine();

        try
        {
            task = Task.Run(() => TaskMethod("Task 2", 2));
            int result = task.GetAwaiter().GetResult();
            Console.WriteLine("Result: {0}", result);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception caught: {0}", e.Message);
        }
        Console.WriteLine("----------------------------------------");
        Console.WriteLine();

        var t1 = new Task<int>(() => TaskMethod("Task 3", 3));
        var t2 = new Task<int>(() => TaskMethod("Task 4", 2));

        var complexTask = Task.WhenAll(t1, t2);

        var exceptionHandler = complexTask.ContinueWith(t =>
            Console.WriteLine("Exception caught: {0}", t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        t1.Start();
        t2.Start();

        Thread.Sleep(TimeSpan.FromSeconds(5));
    }

    private static int TaskMethod(string name, int seconds)
    {
        Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
            name, Environment.CurrentManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);

        Thread.Sleep(TimeSpan.FromSeconds(seconds));
        throw new Exception("Boom!");
        return 42 * seconds;
    }
}