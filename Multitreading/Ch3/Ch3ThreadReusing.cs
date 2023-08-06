namespace dotnet7_research.Multitreading.Ch3;

public static class Ch3ThreadReusing
{
    public static void Start()
    {
        const int x = 1;
        const int y = 2;
        const string lambdaState = "lambda state 2";

        ThreadPool.QueueUserWorkItem(AsyncOperation);
        Thread.Sleep(TimeSpan.FromSeconds(1));
        Console.WriteLine("-------------------------------------");

        ThreadPool.QueueUserWorkItem(AsyncOperation, "async state");
        Thread.Sleep(TimeSpan.FromSeconds(1));
        Console.WriteLine("-------------------------------------");

        ThreadPool.QueueUserWorkItem(state =>
        {
            Console.WriteLine("Is a background thread: {0}", Thread.CurrentThread.IsBackground);
            Console.WriteLine("Operation state: {0}", state);
            Console.WriteLine("Worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }, "lambda state");
        Console.WriteLine("-------------------------------------");

        ThreadPool.QueueUserWorkItem(_ =>
        {
            Console.WriteLine("Is a background thread: {0}", Thread.CurrentThread.IsBackground);
            Console.WriteLine("Operation state: {0}, {1}", x + y, lambdaState);
            Console.WriteLine("Worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }, "lambda state");
        Thread.Sleep(TimeSpan.FromSeconds(2));
        Console.WriteLine("-------------------------------------");
    }

    private static void AsyncOperation(object? state)
    {
        Console.WriteLine("Is a background thread: {0}", Thread.CurrentThread.IsBackground);
        Console.WriteLine("Operation state: {0}", state ?? "(null)");
        Console.WriteLine("Worker thread id: {0}", Thread.CurrentThread.ManagedThreadId);
        //Thread.Sleep(TimeSpan.FromSeconds(2));
    }
}