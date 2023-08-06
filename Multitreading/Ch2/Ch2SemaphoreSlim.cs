using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace dotnet7_research.Multitreading.Ch2;

public static class Ch2SemaphoreSlim
{
    static readonly SemaphoreSlim _semaphore = new(1);

    public static void Start()
    {
        for (var i = 0; i < 4; i++)
        {
            var threadName = $"Thread {i}";
            var secondsToWait = 2 + 2 * i;
            var t = new Thread(() => AccessDatabase(threadName, secondsToWait));
            t.Start();
        }
    }

    private static void AccessDatabase(string name, int seconds)
    {
        //Console.WriteLine("CURRENT COUNT = {0}", _semaphore.CurrentCount);
        Console.WriteLine("{0} waits to access a database", name);
        _semaphore.Wait(TimeSpan.FromSeconds(10));
        Console.WriteLine("{0} was granted an access a database", name);
        Thread.Sleep(TimeSpan.FromSeconds(3));
        //Console.WriteLine("{0} is completed", name);
        _semaphore.Release();
    }

    private static async Task AccessDatabaseAsync(string name, int seconds)
    {
        //Console.WriteLine("CURRENT COUNT = {0}", _semaphore.CurrentCount);
        Console.WriteLine("{0} waits to access a database", name);
        var res = await _semaphore.WaitAsync(TimeSpan.FromSeconds(10));
        Console.WriteLine("{0} was granted an access a database", name);
        Thread.Sleep(TimeSpan.FromSeconds(3));
        //Console.WriteLine("{0} is completed", name);
        _semaphore.Release();
    }
}

public class C
{
    async Task Do1(int i)
    {
        Console.WriteLine("*" + i);
        await Task.Delay(100);
        Console.WriteLine("**" + i);
    }

    void DoANother2(int i)
    {
        Console.WriteLine(i);
    }
    public B Do(int i)
    {
        void DoANother()
        {
            Console.WriteLine(i);
        }

        return new B(() => DoANother2(i), () => Do1(i));
    }
}

public class B
{
    public Action A;
    public Func<Task> T;

    public B(Action a, Func<Task> t)
    {
        A = a;
        T = t;
    }

    public async Task Do()
    {
        A();
        await T();
    }
}