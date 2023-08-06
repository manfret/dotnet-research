﻿namespace dotnet7_research.Multitreading.Ch4;

public static class Ch4Task
{
    public static void Start()
    {
        var t1 = new Task(() => TaskMethod("Task 1"));
        var t2 = new Task(() => TaskMethod("Task 2"));

        t2.Start();
        t1.Start();

        Task.Run(() => TaskMethod("Task 3"));
        Task.Factory.StartNew(() => TaskMethod("Task 4"));
        Task.Factory.StartNew(() => TaskMethod("Task 5"), TaskCreationOptions.LongRunning);
        Thread.Sleep(TimeSpan.FromSeconds(1));
    }

    private static void TaskMethod(string name)
    {
        Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread {2}", 
            name, Environment.CurrentManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
    }

    private static Task TaskMethod1(string name)
    {
        Console.WriteLine("Task {0} is running on a thread id {1}. Is thread pool thread {2}",
            name, Environment.CurrentManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
        return Task.CompletedTask;
    }
}