namespace dotnet7_research.Multitreading.Ch4;

public static class Ch4Cancellation
{
    public static void Start()
    {
        var cts = new CancellationTokenSource();
        //cts.Token в конструкторе таска для того чтобы отменять НЕ НАЧАТЫЕ таски
        //если таск начать после отмены - ошибка
        var longTask = new Task<int>(() => TaskMethod("Task 1", 10, cts.Token), cts.Token);
        Console.WriteLine(longTask.Status);
        cts.Cancel();
        Console.WriteLine(longTask.Status);
        Console.WriteLine("First task has been cancelled before execution");

        cts = new CancellationTokenSource();
        longTask = new Task<int>(() => TaskMethod("Task 2", 10, cts.Token), cts.Token);
        longTask.Start();
        for (var i = 0; i < 5; i++)
        {
            Thread.Sleep(TimeSpan.FromSeconds(0.5));
            Console.WriteLine(longTask.Status);
        }
        cts.Cancel();
        for (var i = 0; i < 5; i++)
        {
            Thread.Sleep(TimeSpan.FromSeconds(0.5));
            Console.WriteLine(longTask.Status);
        }
        Console.WriteLine("A task has been completed with result: {0}", longTask.Result);
    }

    private static int TaskMethod(string name, int seconds, CancellationToken token)
    {
        Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
            name, Environment.CurrentManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);

        for (var i = 0; i < seconds; i++)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            if (token.IsCancellationRequested) return -1;
        }
        return 42 * seconds;
    }
}