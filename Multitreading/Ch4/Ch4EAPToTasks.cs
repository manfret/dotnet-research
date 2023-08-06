using System.ComponentModel;

namespace dotnet7_research.Multitreading.Ch4;

public static class Ch4EAPToTasks
{
    public static void Start()
    {
        var tcs = new TaskCompletionSource<int>();

        var worker = new BackgroundWorker();
        worker.DoWork += (sender, eventArgs) =>
        {
            eventArgs.Result = TaskMethod("Backgroun worker", 5);
        };
        worker.RunWorkerCompleted += (sender, eventArgs) =>
        {
            if (eventArgs.Error != null)
            {
                tcs.SetException(eventArgs.Error);
            }
            else if (eventArgs.Cancelled)
            {
                tcs.SetCanceled();
            }
            else
            {
                tcs.SetResult((int)eventArgs.Result!);
            }
        };

        worker.RunWorkerAsync();

        var result = tcs.Task.Result;

        Console.WriteLine("Result is: {0}", result);

    }

    private static int TaskMethod(string name, int seconds)
    {
        Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
            name, Environment.CurrentManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
        return 42;
    }
}