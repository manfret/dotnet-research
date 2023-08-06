namespace dotnet7_research.Multitreading.Ch7;

//через Parallel удобно запускать циклы и множество операций без ожидания окончания и получения результата
public static class Ch7Parallel
{
    public static void Start()
    {
        Parallel.Invoke(
            () => EmulateProcessing("Task1"),
            () => EmulateProcessing("Task2"),
            () => EmulateProcessing("Task3")
        );

        var cts = new CancellationTokenSource();
        var result = Parallel.ForEach(
            Enumerable.Range(1, 30),
            new ParallelOptions
            {
                CancellationToken = cts.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                TaskScheduler = TaskScheduler.Default
            },
            (i, state) =>
            {
                Console.WriteLine(i);
                if (i == 20)
                {
                    state.Break();
                    Console.WriteLine("Loop is stopped: {0}", state.IsStopped);
                }
            });

        Console.WriteLine("---");
        Console.WriteLine("IsCompleted: {0}", result.IsCompleted);
        Console.WriteLine("Lowest break iteration: {0}", result.LowestBreakIteration);
    }

    private static string EmulateProcessing(string taskName)
    {
        Thread.Sleep(TimeSpan.FromMicroseconds(
            new Random(DateTime.Now.Millisecond).Next(250, 350)));
        Console.WriteLine("{0} task was processed on a thread id {1}", 
            taskName, Environment.CurrentManagedThreadId);
        return taskName;
    }
}