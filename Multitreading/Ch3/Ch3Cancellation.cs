namespace dotnet7_research.Multitreading.Ch3;

public static class Ch3Cancellation
{
    public static void Start()
    {
        using (var cts = new CancellationTokenSource())
        {
            var token = cts.Token;
            ThreadPool.QueueUserWorkItem(_ => AsyncOperation1(token));
            Thread.Sleep(TimeSpan.FromSeconds(2));
            cts.Cancel();
        }

        using (var cts = new CancellationTokenSource())
        {
            var token = cts.Token;
            ThreadPool.QueueUserWorkItem(_ => AsyncOperation2(token));
            Thread.Sleep(TimeSpan.FromSeconds(2));
            cts.Cancel();
        }

        using (var cts = new CancellationTokenSource())
        {
            var token = cts.Token;
            ThreadPool.QueueUserWorkItem(_ => AsyncOperation3(token));
            Thread.Sleep(TimeSpan.FromSeconds(2));
            cts.Cancel();
        }

        Thread.Sleep(TimeSpan.FromSeconds(2));
    }

    private static void AsyncOperation1(CancellationToken token)
    {
        Console.WriteLine("Starting the first task");
        for (var i = 0; i < 5; i++)
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("The first task has been cancelled");
                return;
            }
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        Console.WriteLine("The first task has completed sucessfully");
    }

    private static void AsyncOperation2(CancellationToken token)
    {
        try
        {
            Console.WriteLine("Starting the second task");
            for (var i = 0; i < 5; i++)
            {
                token.ThrowIfCancellationRequested();
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            Console.WriteLine("The second task has completed sucessfully");
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine("The second task has been cancelled");
        }
    }

    private static void AsyncOperation3(CancellationToken token)
    {
        var cancellationFlag = false;
        token.Register(() => cancellationFlag = true);
        Console.WriteLine("Starting the third task");
        for (var i = 0; i < 5; i++)
        {
            if (cancellationFlag)
            {
                Console.WriteLine("The third task has been cancelled");
                return;
            }
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        Console.WriteLine("The third task has completed sucessfully");
    }
}