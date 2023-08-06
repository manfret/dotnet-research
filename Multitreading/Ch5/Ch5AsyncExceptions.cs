namespace dotnet7_research.Multitreading.Ch5;

public static class Ch5AsyncExceptions
{
    public static void Start()
    {
        Task t = AsyncronousProcessing();
        t.Wait();
    }

    private static async Task AsyncronousProcessing()
    {
        Console.WriteLine("1. Single exceptions");

        try
        {
            string result = await GetInfoAsync("Task 1", 2);
            Console.WriteLine(result);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception details: {0}", e is AggregateException);
        }

        Console.WriteLine();
        Console.WriteLine("2. Multiple exceptions");

        Task<string> t1 = GetInfoAsync("Task 1", 3);
        Task<string> t2 = GetInfoAsync("Task 2", 2);
        try
        {
            string[] results = await Task.WhenAll(t1, t2);
            Console.WriteLine(results.Length);
        }
        //только первое исключение на самом деле(первой задачи)
        catch (Exception e)
        {
            Console.WriteLine("Exception details: {0}", e);

        }

        Console.WriteLine();
        Console.WriteLine("3. Multiple exceptions with AggregateException");

        t1 = GetInfoAsync("Task 1", 3);
        t2 = GetInfoAsync("Task 2", 2);
        Task<string[]> t3 = Task.WhenAll(t1, t2);

        try
        {
            string[] results = await t3;
            Console.WriteLine(results.Length);
        }
        catch
        {
            var ae = t2.Exception.Flatten();
            var exceptions = ae.InnerExceptions;
            Console.WriteLine("Exceptions caught: {0} for type: {1}", exceptions.Count, t2.Exception.GetType());
            foreach (var e in exceptions)
            {
                Console.WriteLine("Exception details: {0}", e);
                Console.WriteLine();
            }
        }
    }

    private static async Task<string> GetInfoAsync(string name, int seconds)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
        Console.WriteLine(name);
        throw new Exception($"Boom from {name}!");
    }
}