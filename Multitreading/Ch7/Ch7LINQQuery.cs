using System.Diagnostics;

namespace dotnet7_research.Multitreading.Ch7;

//PLinq - можно распараллелить IEnumerable (итемы будут передаваться в разные потоки)
//по дефолту - select - в одном потоке (foreach), но можно сделать и в нескольких (parallelQuery.ForAll)
public static class Ch7LINQQuery
{
    public static void Start()
    {
        var sw = new Stopwatch();
        sw.Start();
        var query = from t in GetTypes()
            select EmulateProcessing(t);
        foreach (var typeName in query)
        {
            PrintInfo(typeName);
        }
        sw.Stop();
        Console.WriteLine("---");
        Console.WriteLine("Sequential LINQ query.");
        Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
        Console.WriteLine("Press ENTER to continue...");
        Console.ReadLine();

        Console.Clear();
        sw.Reset();
        sw.Start();
        var parallelQuery = 
            from t in ParallelEnumerable.AsParallel(GetTypes())
            select EmulateProcessing(t);
        foreach (var typeName in parallelQuery)
        {
            PrintInfo(typeName);
        }
        sw.Stop();
        Console.WriteLine("---");
        Console.WriteLine("Parallel LINQ query. The result are being merged on a single thread");
        Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
        Console.WriteLine("Press ENTER to continue...");
        Console.ReadLine();

        Console.Clear();
        sw.Reset();
        sw.Start();
        parallelQuery =
            from t in GetTypes().AsParallel()
            select EmulateProcessing(t);
        parallelQuery.ForAll(PrintInfo);
        sw.Stop();
        Console.WriteLine("---");
        Console.WriteLine("Parallel LINQ query. The result are being processed in parallel");
        Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
        Console.WriteLine("Press ENTER to continue...");
        Console.ReadLine();

        Console.Clear();
        sw.Reset();
        sw.Start();
        query = from t in GetTypes().AsParallel().AsSequential()
            select EmulateProcessing(t);
        foreach (var typeName in query)
        {
            PrintInfo(typeName);
        }
        sw.Stop();
        Console.WriteLine("---");
        Console.WriteLine("Parallel LINQ query, transformed into sequential.");
        Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
        Console.WriteLine("Press ENTER to continue...");
        Console.ReadLine();
        Console.Clear();
    }

    private static IEnumerable<string> GetTypes()
    {
        return from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetExportedTypes()
            where type.Name.StartsWith("System")
            select type.Name;
    }

    private static string EmulateProcessing(string typeName)
    {
        Thread.Sleep(TimeSpan.FromMicroseconds(150));
        Console.WriteLine("{0} task was processed on a thread id {1}",
            typeName, Environment.CurrentManagedThreadId);
        return typeName;
    }

    private static void PrintInfo(string typeName)
    {
        Thread.Sleep(TimeSpan.FromMicroseconds(150));
        Console.WriteLine("{0} task was printed on a thread id {1}",
            typeName, Environment.CurrentManagedThreadId);
    }
}