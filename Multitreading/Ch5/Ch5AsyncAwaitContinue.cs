namespace dotnet7_research.Multitreading.Ch5;

public static class Ch5AsyncAwaitContinue
{
    public static void Start()
    {
        Task t = AsynhcronyWithTPL();
        t.Wait();

        t = AsynhcronyWithAwait();
        t.Wait();
    }

    private static Task AsynhcronyWithTPL()
    {
        var containerTask = new Task(() =>
        {
            Task<string> t = GetInfoAsync("TPL 1");
            t.ContinueWith(task =>
            {
                Console.WriteLine(task.Result);
                Task<string> t2 = GetInfoAsync("TPL 2");
                t2.ContinueWith(innerTask =>
                {
                    Console.WriteLine(innerTask.Result);
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted);
                t2.ContinueWith(innerTask =>
                {
                    Console.WriteLine(innerTask.Exception.InnerExceptions);
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);
            }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted);
            t.ContinueWith(task =>
            {
                Console.WriteLine(task.Exception.InnerExceptions);
            }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);
        });

        containerTask.Start();
        return containerTask;
    }

    private static async Task AsynhcronyWithAwait()
    {
        try
        {
            string result = await GetInfoAsync("Async 1");
            Console.WriteLine(result);
            result = await GetInfoAsync("Async 2");
            Console.WriteLine(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task<string> GetInfoAsync(string name)
    {
        Console.WriteLine("Task {0} started!", name);
        await Task.Delay(TimeSpan.FromSeconds(2));
        if (name == "TPL 2") throw new Exception("Boom!");
        return $"Task {name} is running on a thread id {Environment.CurrentManagedThreadId}. " +
               $"Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}";
    }
}