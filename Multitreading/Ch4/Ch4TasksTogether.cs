namespace dotnet7_research.Multitreading.Ch4;

public static class Ch4TasksTogether
{
    public static void Start()
    {
        var firstTask = new Task<int>(() => TaskMethod("First Task", 3));
        var secondTask = new Task<int>(() => TaskMethod("Second Task", 2));

        firstTask.ContinueWith(
            t => Console.WriteLine("The first answer is {0}. Thread id {1}, is thread pool thread: {2}, status = {3}",
                t.Result, Environment.CurrentManagedThreadId, Thread.CurrentThread.IsThreadPoolThread, t.Status),
            TaskContinuationOptions.OnlyOnRanToCompletion);

        firstTask.Start();
        secondTask.Start();

        Thread.Sleep(TimeSpan.FromSeconds(4));

        //сразу выполнено IsThreadPoolThread = False т.к. можно выполнять на основном потоке
        var continuation = secondTask.ContinueWith(
            t => Console.WriteLine("The second answer is {0}. Thread id {1}, is threas pool thread: {2}, status = {3}",
                t.Result, Environment.CurrentManagedThreadId, Thread.CurrentThread.IsThreadPoolThread, t.Status),
            TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);

        continuation.GetAwaiter().OnCompleted(
            () => Console.WriteLine("Continuation Task Completed! Thread id: {0}, is thread pool thread: {1}",
                Environment.CurrentManagedThreadId, Thread.CurrentThread.IsThreadPoolThread));

        Thread.Sleep(TimeSpan.FromSeconds(2));
        Console.WriteLine();

        firstTask = new Task<int>(() =>
        {
            var innerTask = Task.Factory.StartNew(() => TaskMethod("Second task", 5), TaskCreationOptions.AttachedToParent);
            innerTask.ContinueWith(t => TaskMethod("Third task", 2), TaskContinuationOptions.AttachedToParent);
            return TaskMethod("First task", 2);
        });

        firstTask.Start();

        while (!firstTask.IsCompleted)
        {
            Console.WriteLine(firstTask.Status);
            Thread.Sleep(TimeSpan.FromSeconds(0.5));
        }
        Console.WriteLine(firstTask.Status);

        Thread.Sleep(TimeSpan.FromSeconds(10));
    }

    private static int TaskMethod(string name, int seconds)
    {
        Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
            name, Environment.CurrentManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
        return 42;
    }
}