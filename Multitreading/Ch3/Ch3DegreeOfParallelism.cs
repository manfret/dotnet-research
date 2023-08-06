using System.Diagnostics;

namespace dotnet7_research.Multitreading.Ch3;

public static class Ch3DegreeOfParallelism
{
    public static void Start()
    {
        const int numberOfOperations = 500;
        var sw = new Stopwatch();
        sw.Start();
        UseThreads(numberOfOperations);
        sw.Stop();
        Console.WriteLine("Execution time using threads: {0}", sw.ElapsedMilliseconds);

        sw.Reset();
        sw.Start();
        UseThreadPool(numberOfOperations);
        sw.Stop();
        Console.WriteLine("Execution time using thread pool: {0}", sw.ElapsedMilliseconds);
    }

    private static void UseThreads(int numberOfOperations)
    {
        using (var countdown = new CountdownEvent(numberOfOperations))
        {
            Console.WriteLine("Scheduling work by creating threads");
            for (var i = 0; i < numberOfOperations; i++)
            {
                var thread = new Thread(() =>
                {
                    Console.WriteLine("{0}", Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(TimeSpan.FromSeconds(0.1));
                    countdown.Signal();
                });
                thread.Start();
            }

            countdown.Wait();
            Console.WriteLine();
        }
    }
    private static void UseThreadPool(int numberOfOperations)
    {
        using (var countdown = new CountdownEvent(numberOfOperations))
        {
            Console.WriteLine("Scheduling work on threadpool");
            for (var i = 0; i < numberOfOperations; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Console.WriteLine("{0}", Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(TimeSpan.FromSeconds(0.1));
                    countdown.Signal();
                });
            }

            countdown.Wait();
            Console.WriteLine();
        }
    }
}