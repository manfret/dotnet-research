namespace dotnet7_research.Multitreading.Ch3;

public static class Ch3WaitHandle
{
    public static void Start()
    {
        RunOperations(TimeSpan.FromSeconds(5));
        RunOperations(TimeSpan.FromSeconds(7));
    }

    private static void RunOperations(TimeSpan workerOperationTimeout)
    {
        using (var evt = new ManualResetEvent(false))
        {
            using (var cts = new CancellationTokenSource())
            {
                Console.WriteLine("Registering timeout operations...");
                var worker = ThreadPool.RegisterWaitForSingleObject(
                    evt, (_, isTimeOut) => WorkerOperationWait(cts, isTimeOut), null, workerOperationTimeout, true);
                Console.WriteLine("Starting long running operation...");

                ThreadPool.QueueUserWorkItem(_ => WorkerOperation(cts.Token, evt));

                Thread.Sleep(workerOperationTimeout.Add(TimeSpan.FromSeconds(2)));
                worker.Unregister(evt);
            }
        }
    }

    private static void WorkerOperation(CancellationToken token, ManualResetEvent evt)
    {
        for (var i = 0; i < 6; i++)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        evt.Set();
    }

    private static void WorkerOperationWait(CancellationTokenSource cts, bool isTimeOut)
    {
        if (isTimeOut)
        {
            cts.Cancel();
            Console.WriteLine("Worker operation times out and was cancelled");
        }
        else
        {
            Console.WriteLine("Worker operation succeded");
        }
    }
}