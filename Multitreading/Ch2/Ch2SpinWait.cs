namespace dotnet7_research.Multitreading.Ch2;

public class Ch2SpinWait
{
    private static volatile bool _isCompleted = false;

    public static void Start()
    {
        var t1 = new Thread(UserModeWait);
        var t2 = new Thread(HybridSpinWait);

        Console.WriteLine("Running user mode waiting");
        t1.Start();
        Thread.Sleep(20);
        _isCompleted = true;
        Thread.Sleep(TimeSpan.FromSeconds(1));
        _isCompleted = false;
        Console.WriteLine("Running hybrid mode");
        t2.Start();
        Thread.Sleep(5);
        Console.WriteLine("End");
        _isCompleted = true;
    }

    private static void UserModeWait()
    {

        while (!_isCompleted)
        {
            Console.Write(".");
            //Thread.Sleep(100);
        }
        Console.WriteLine();
        Console.WriteLine("Waiting is complete");
    }

    private static void HybridSpinWait()
    {
        var w = new SpinWait();
        while (!_isCompleted)
        {
            w.SpinOnce();
            Console.WriteLine(w.NextSpinWillYield);
            //Thread.Sleep(100);
        }
        Console.WriteLine("Waiting is complete");
    }
}