namespace dotnet7_research.Multitreading.Ch7;

public static class Ch7SetupPLINQQuery
{
    public static void Start()
    {
        var parallelQuery =
            from t in GetTypes().AsParallel()
            select EmulateProcessing(t);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(3));

        try
        {
            parallelQuery.WithDegreeOfParallelism(Environment.ProcessorCount)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithMergeOptions(ParallelMergeOptions.Default)
                .WithCancellation(cts.Token)
                .ForAll(Console.WriteLine);
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine("-----");
            Console.WriteLine("Operation has been cancelled");
        }

        Console.WriteLine("-----");
        Console.WriteLine("Unordered PLINQ query execution");
        var unorderedQuery =
            from i in ParallelEnumerable.Range(1, 30)
            select i;
        foreach (var i in unorderedQuery)
        {
            Console.WriteLine(i);
        }
        Console.WriteLine("-----");
        Console.WriteLine("Ordered PLINQ query execution");
        var orderedQuery =
            from i in ParallelEnumerable.Range(1, 30).AsOrdered()
            select i;
        foreach (var i in orderedQuery)
        {
            Console.WriteLine(i);
        }
    }

    private static IEnumerable<string> GetTypes()
    {
        return from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetExportedTypes()
            where type.Name.StartsWith("System")
            orderby type.Name.Length
            select type.Name;
    }

    private static string EmulateProcessing(string typeName)
    {
        Thread.Sleep(TimeSpan.FromMicroseconds(
            new Random(DateTime.Now.Millisecond).Next(250, 350)));
        Console.WriteLine("{0} task was processed on a thread id {1}",
            typeName, Environment.CurrentManagedThreadId);
        return typeName;
    }
}