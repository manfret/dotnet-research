namespace dotnet7_research.Multitreading.Ch5;

public static class Ch5AsyncAwaitInLambda
{
    public static void Start()
    {
        var t = AsyncronousProcessing();
        t.Wait();
    }

    private static async Task AsyncronousProcessing()
    {
        Func<string, Task<string>> asyncLambda = async name =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            return $"Task {name} is running on a thread id {Environment.CurrentManagedThreadId}." +
                   $"Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}";
        };

        var result = await asyncLambda("async lambda");

        Console.WriteLine(result);
    }
}