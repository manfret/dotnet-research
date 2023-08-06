namespace dotnet7_research.Multitreading.Ch4;

public static class Ch4Continuation
{
    public static void Start()
    {
        var firstTask = new Task<int>(() => TaskMethod("First task", 3));
        var secondTask = new Task<int>(() => TaskMethod("Second task", 2));
        var whenAllTask = Task.WhenAll(firstTask, secondTask);

        whenAllTask.ContinueWith(t =>
                Console.WriteLine("The first answer is {0}, the second is {1}", t.Result[0], t.Result[1]),
            TaskContinuationOptions.OnlyOnRanToCompletion);

        firstTask.Start();
        secondTask.Start();

        Thread.Sleep(TimeSpan.FromSeconds(4));

        var tasks = new List<Task<int>>();
        for (var i = 0; i < 4; i++)
        {
            var counter = i;
            var task = new Task<int>(() => TaskMethod($"Task {counter}", counter));
            tasks.Add(task);
            task.Start();
        }

        while (tasks.Count > 0)
        {
            var completedTask = Task.WhenAny(tasks).Result;
            tasks.Remove(completedTask);
            Console.WriteLine("A task has been completed with result {0}.", completedTask.Result);
        }

        Thread.Sleep(TimeSpan.FromSeconds(1));
    }

    private static int TaskMethod(string name, int seconds)
    {
        Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}",
            name, Environment.CurrentManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);

        Thread.Sleep(TimeSpan.FromSeconds(seconds));
        return 42 * seconds;
    }
}