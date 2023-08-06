using Microsoft.CodeAnalysis;
using System;

namespace dotnet7_research;

public class IncrementDemo
{
    private int _counter = 0;
    private ulong _counterU = 0;
    private readonly object _syncObj = new object();
    private Timer _timer;

    public async void DoIter(object? sender)
    {
        await InterlockedExchangeIfGreaterThan();
        //await InterlockedExchangeIfGreaterThan2();

        /*
        var id = Guid.NewGuid();
        Console.WriteLine($"1 id = {id}  c = {_counter}");
        if (_counter >= 1) return;
        Console.WriteLine($"2 id = {id}  c = {_counter}");
        if (Interlocked.CompareExchange(ref _counter, _counter + 1, _counter) == _counter) return;
        Console.WriteLine($"3 id = {id} c = {_counter}");
        await Task.Delay(2);
        Interlocked.Decrement(ref _counter);
        */
    }

    public async Task InterlockedExchangeIfGreaterThan()
    {
        var id = Guid.NewGuid();
        lock (_syncObj)
        {
            var initial = _counter;
            Console.WriteLine($"1 id = {id} c = {initial}");
            if (initial >= 3) return;
        }
        Interlocked.Increment(ref _counter);
        Console.WriteLine($"2 id = {id} c = {_counter}");
        await Task.Delay(500);
        Interlocked.Decrement(ref _counter);
        Console.WriteLine($"3 id = {id} c = {_counter}");
    }

    public async Task InterlockedExchangeIfGreaterThan2()
    {
        lock (_syncObj)
        {
            if ((ulong)3 > Interlocked.Read(ref _counterU))
            {
                Interlocked.Increment(ref _counter);
                Console.WriteLine($"c = {_counter}");
            }
        }

        await Task.Delay(100);
        Interlocked.Decrement(ref _counter);
    }

    public async Task RunIt()
    {
        _timer = new Timer(DoIter, null, 0, 1);
        await Task.Delay(2000);
    }
}