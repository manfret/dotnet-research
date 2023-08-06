namespace dotnet7_research.Multitreading.Ch2;

public class Ch2ReaderWriterLockSlim
{
    private static ReaderWriterLockSlim _rw = new ReaderWriterLockSlim();
    private static Dictionary<int, int> _items = new Dictionary<int, int>();

    public static void Start()
    {
        var t1 = new Thread(Read) { IsBackground = true };
        var t2 = new Thread(Read) { IsBackground = true };
        var t3 = new Thread(Read) { IsBackground = true };
        var t4 = new Thread(Read) { IsBackground = true };

        t1.Start();
        t2.Start();
        t3.Start();
        t4.Start();

        var t5 = new Thread(() => Write("Thread 5")) { IsBackground = true };
        var t6 = new Thread(() => Write("Thread 6")) { IsBackground = true };

        t5.Start();
        t6.Start();

        Thread.Sleep(TimeSpan.FromSeconds(30));
    }

    private static void Read()
    {
        //Thread.Sleep(TimeSpan.FromSeconds(1));
        Console.WriteLine("Reading content of a dictionary");
        while (true)
        {
            try
            {
                _rw.EnterReadLock();
                foreach (var key in _items.Keys)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(0.1));
                }
            }
            finally
            {
                _rw.ExitReadLock();
            }
        }
    }

    private static void Write(string threadName)
    {
        while (true)
        {
            try
            {
                var newKey = new Random().Next(250);
                _rw.EnterUpgradeableReadLock();
                if (!_items.ContainsKey(newKey))
                {
                    try
                    {
                        _rw.EnterWriteLock();
                        _items[newKey] = 1;
                        Console.WriteLine("New key {0} is added to a dictionary by a {1}", newKey, threadName);
                    }
                    finally
                    {
                        _rw.ExitWriteLock();
                    }
                }
                Thread.Sleep(TimeSpan.FromSeconds(0.1));
            }
            finally
            {
                _rw.ExitUpgradeableReadLock();
            }

        }
    }
}