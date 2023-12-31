﻿namespace dotnet7_research.Multitreading.Ch2;

public class Ch2ManualResetEventSlim
{
    private static readonly ManualResetEventSlim _mainEvent = new ManualResetEventSlim(false);

    public static void Start()
    {
        var t1 = new Thread(() => TravelThroughtGates("Thread 1", 5));
        var t2 = new Thread(() => TravelThroughtGates("Thread 2", 6));
        var t3 = new Thread(() => TravelThroughtGates("Thread 3", 12));

        t1.Start();
        t2.Start();
        t3.Start();

        Thread.Sleep(TimeSpan.FromSeconds(6));
        Console.WriteLine("The gates are now open!");
        _mainEvent.Set();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        _mainEvent.Reset();
        Console.WriteLine("The gates have been closed!");
        Thread.Sleep(TimeSpan.FromSeconds(10));
        Console.WriteLine("The gates are now open for the second time!");
        _mainEvent.Set();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        Console.WriteLine("The gates have been closed!");
        _mainEvent.Reset();
    }

    private static void TravelThroughtGates(string threadName, int seconds)
    {
        Console.WriteLine("{0} falls to sleep", threadName);
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
        Console.WriteLine("{0} waits for the gates to open!", threadName);
        _mainEvent.Wait();
        Console.WriteLine("{0} enters to gates!", threadName);
    }
}