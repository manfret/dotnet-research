namespace dotnet7_research.Multitreading.Ch2;

public static class Ch2Atomic
{
    public static void Start()
    {
        Console.WriteLine("Incorrect counter");

        var c = new Counter();

        var t1 = new Thread(() => TestCounter(c));
        var t2 = new Thread(() => TestCounter(c));
        var t3 = new Thread(() => TestCounter(c));
        
        t1.Start();
        t2.Start();
        t3.Start();

        t1.Join();
        t2.Join();
        t3.Join();

        Console.WriteLine("Total count = {0}", c.Count);
        Console.WriteLine("---------------------------------");
        Console.WriteLine("Correct counter");

        var c1 = new CounterNoLock();

        t1 = new Thread(() => TestCounter(c1));
        t2 = new Thread(() => TestCounter(c1));
        t3 = new Thread(() => TestCounter(c1));

        t1.Start();
        t2.Start();
        t3.Start();

        t1.Join();
        t2.Join();
        t3.Join();

        Console.WriteLine("Total count = {0}", c1.Count);
    }


    public static void TestCounter(CounterBase c)
    {
        for (var i = 0; i < 100000; i++)
        {
            c.Increment();
            c.Decrement();
        }
    }
}

public abstract class CounterBase
{
    public abstract void Increment();
    public abstract void Decrement();
}

public class CounterNoLock : CounterBase
{
    private int _count;
    public int Count => _count;

    public override void Increment()
    {
        Interlocked.Increment(ref _count);
    }

    public override void Decrement()
    {
        Interlocked.Decrement(ref _count);
    }
}

public class Counter : CounterBase
{
    private int _count;
    public int Count => _count;

    public override void Increment()
    {
        _count++;
    }

    public override void Decrement()
    {
        _count--;
    }
}