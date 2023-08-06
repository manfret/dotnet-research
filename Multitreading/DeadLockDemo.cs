namespace dotnet7_research.Multitreading;

public class DeadLockDemo
{
    public static void Start()
    {
        var lock1 = new object();
        var lock2 = new object();

        var t1 = new Thread(() => LockTooMuch(lock1, lock2));
        t1.Start();

        lock (lock2)
        {
            Thread.Sleep(1000);
            Console.WriteLine("Monitor.TryEnter allows not to get stuck");
            //ждем здесь пока разблочится lock1 и поэтому не отпускаем lock2
            if (Monitor.TryEnter(lock1, TimeSpan.FromSeconds(5)))
            {
                Console.WriteLine("Acquired a protected resource sucessfully");
            }
            else
            {
                Console.WriteLine("Timeout acquiring a resource");
            }
        }

        Console.WriteLine("-----------------------------------");
        var t2 = new Thread(() => LockTooMuch(lock1, lock2));
        lock (lock2)
        {
            Console.WriteLine("This will be a deadlock");
            Thread.Sleep(1000);
            lock (lock1)
            {
                Console.WriteLine("Acquired a protected resource sucessfully");
            }
        }
    }

    private static void LockTooMuch(object lock1, object lock2)
    {
        lock (lock1)
        {
            Thread.Sleep(1000);
            //ждем здесь пока разблочится lock2 и из-за этого не отпускаем lock 1
            lock (lock2)
            {
            }
        }
    }
}