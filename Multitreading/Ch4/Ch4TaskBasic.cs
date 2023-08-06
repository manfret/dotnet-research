namespace dotnet7_research.Multitreading.Ch4;

public static class Ch4TaskBasic
{
    public static void Start()
    {
        TaskMethod("Main Thread Task");

        var task = CreateTask("Task 1");
        task.Start();
        var result = task.Result;
        Console.WriteLine("Result is: {0}", result);

        task = CreateTask("Task 2");
        task.RunSynchronously();
        result = task.Result;
        Console.WriteLine("Result is: {0}", result);

        task = CreateTask("Task 3");
        task.Start();
        while (!task.IsCompleted)
        {
            Console.WriteLine(task.Status);
            Thread.Sleep(TimeSpan.FromSeconds(0.5));
        }
        Console.WriteLine(task.Status);
        result = task.Result;
        Console.WriteLine("Result is: {0}", result);
    }

    private static Task<int> CreateTask(string name)
    {
        return new Task<int>(() => TaskMethod(name));
    }

    private static int TaskMethod(string name)
    {
        Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread: {2}", 
            name, Environment.CurrentManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        Thread.Sleep(TimeSpan.FromSeconds(2));
        return 42;
    }
}